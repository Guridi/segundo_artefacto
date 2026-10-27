using System.Data;
using CatalogoApartamentosAPI.Interfaces;
using CatalogoApartamentosAPI.Models;
using Microsoft.Data.SqlClient;

namespace CatalogoApartamentosAPI.Services;

public class ResidencialService : IResidencialService
{
    private readonly string _connectionString;

    public ResidencialService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se configuró DefaultConnection.");
    }

    public IEnumerable<Residencial> GetResidenciales()
    {
        var residenciales = new List<Residencial>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "SELECT Id, Nombre, Ubicacion, Descripcion FROM dbo.Residencial ORDER BY Id", connection);
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            residenciales.Add(new Residencial
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Ubicacion = reader.GetString(2),
                Descripcion = reader.GetString(3)
            });
        }
        return residenciales;
    }

    public Residencial? GetResidencialById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "SELECT Id, Nombre, Ubicacion, Descripcion FROM dbo.Residencial WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        using SqlDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }
        return new Residencial
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Ubicacion = reader.GetString(2),
            Descripcion = reader.GetString(3)
        };
    }

    public int CrearResidencial(Residencial residencial)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "INSERT INTO dbo.Residencial (Nombre, Ubicacion, Descripcion) " +
            "OUTPUT INSERTED.Id VALUES (@Nombre, @Ubicacion, @Descripcion)", connection);
        command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 150).Value = residencial.Nombre;
        command.Parameters.Add("@Ubicacion", SqlDbType.NVarChar, 250).Value = residencial.Ubicacion;
        command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value = residencial.Descripcion;
        return (int)command.ExecuteScalar()!;
    }

    public bool ActualizarResidencial(int id, Residencial residencial)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "UPDATE dbo.Residencial SET Nombre = @Nombre, Ubicacion = @Ubicacion, " +
            "Descripcion = @Descripcion WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 150).Value = residencial.Nombre;
        command.Parameters.Add("@Ubicacion", SqlDbType.NVarChar, 250).Value = residencial.Ubicacion;
        command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value = residencial.Descripcion;
        return command.ExecuteNonQuery() > 0;
    }

    public bool EliminarResidencial(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("DELETE FROM dbo.Residencial WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return command.ExecuteNonQuery() > 0;
    }

    public bool TieneApartamentos(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "SELECT COUNT(1) FROM dbo.Apartamento WHERE ResidencialId = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return (int)command.ExecuteScalar()! > 0;
    }
}
