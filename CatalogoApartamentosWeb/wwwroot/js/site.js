// Funciones JavaScript invocadas desde Blazor mediante IJSRuntime.
window.catalogo = {
    volverArriba: function () {
        window.scrollTo({ top: 0, behavior: "smooth" });
    },

    mostrarToast: function (mensaje) {
        const toast = document.getElementById("toast-catalogo");
        if (!toast) {
            return;
        }

        toast.textContent = mensaje;
        toast.classList.add("visible");

        clearTimeout(toast.temporizador);
        toast.temporizador = setTimeout(function () {
            toast.classList.remove("visible");
        }, 3000);
    }
};
