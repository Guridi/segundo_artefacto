using System.Data;
using CatalogoApartamentosAPI.Interfaces;
using CatalogoApartamentosAPI.Models;
using Microsoft.Data.SqlClient;

namespace CatalogoApartamentosAPI.Services;

public class ApartamentoService : IApartamentoService
{
    private readonly string _connectionString;

    public ApartamentoService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se configuró DefaultConnection.");
    }

    public IEnumerable<Apartamento> GetApartamentos()
    {
        var apartamentos = new List<Apartamento>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "SELECT Id, Numero, Habitaciones, Banos, Precio, Disponible, ResidencialId " +
            "FROM dbo.Apartamento ORDER BY Id", connection);
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            apartamentos.Add(new Apartamento
            {
                Id = reader.GetInt32(0),
                Numero = reader.GetString(1),
                Habitaciones = reader.GetInt32(2),
                Banos = reader.GetInt32(3),
                Precio = reader.GetDecimal(4),
                Disponible = reader.GetBoolean(5),
                ResidencialId = reader.GetInt32(6)
            });
        }
        return apartamentos;
    }

    public Apartamento? GetApartamentoById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "SELECT Id, Numero, Habitaciones, Banos, Precio, Disponible, ResidencialId " +
            "FROM dbo.Apartamento WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        using SqlDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }
        return new Apartamento
        {
            Id = reader.GetInt32(0),
            Numero = reader.GetString(1),
            Habitaciones = reader.GetInt32(2),
            Banos = reader.GetInt32(3),
            Precio = reader.GetDecimal(4),
            Disponible = reader.GetBoolean(5),
            ResidencialId = reader.GetInt32(6)
        };
    }

    public int CrearApartamento(Apartamento apartamento)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "INSERT INTO dbo.Apartamento (Numero, Habitaciones, Banos, Precio, Disponible, ResidencialId) " +
            "OUTPUT INSERTED.Id VALUES (@Numero, @Habitaciones, @Banos, @Precio, @Disponible, @ResidencialId)",
            connection);
        command.Parameters.Add("@Numero", SqlDbType.NVarChar, 20).Value = apartamento.Numero;
        command.Parameters.Add("@Habitaciones", SqlDbType.Int).Value = apartamento.Habitaciones;
        command.Parameters.Add("@Banos", SqlDbType.Int).Value = apartamento.Banos;
        var precio = command.Parameters.Add("@Precio", SqlDbType.Decimal);
        precio.Precision = 18;
        precio.Scale = 2;
        precio.Value = apartamento.Precio;
        command.Parameters.Add("@Disponible", SqlDbType.Bit).Value = apartamento.Disponible;
        command.Parameters.Add("@ResidencialId", SqlDbType.Int).Value = apartamento.ResidencialId;
        return (int)command.ExecuteScalar()!;
    }

    public bool ActualizarApartamento(int id, Apartamento apartamento)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(
            "UPDATE dbo.Apartamento SET Numero = @Numero, Habitaciones = @Habitaciones, " +
            "Banos = @Banos, Precio = @Precio, Disponible = @Disponible, ResidencialId = @ResidencialId " +
            "WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        command.Parameters.Add("@Numero", SqlDbType.NVarChar, 20).Value = apartamento.Numero;
        command.Parameters.Add("@Habitaciones", SqlDbType.Int).Value = apartamento.Habitaciones;
        command.Parameters.Add("@Banos", SqlDbType.Int).Value = apartamento.Banos;
        var precio = command.Parameters.Add("@Precio", SqlDbType.Decimal);
        precio.Precision = 18;
        precio.Scale = 2;
        precio.Value = apartamento.Precio;
        command.Parameters.Add("@Disponible", SqlDbType.Bit).Value = apartamento.Disponible;
        command.Parameters.Add("@ResidencialId", SqlDbType.Int).Value = apartamento.ResidencialId;
        return command.ExecuteNonQuery() > 0;
    }

    public bool EliminarApartamento(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("DELETE FROM dbo.Apartamento WHERE Id = @Id", connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        return command.ExecuteNonQuery() > 0;
    }
}
