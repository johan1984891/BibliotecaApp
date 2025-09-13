// Funciones globales para la aplicación
(function () {
    // Inicializar tooltips de Bootstrap
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Inicializar popovers de Bootstrap
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Validación personalizada para fechas
    $.validator.addMethod("fechaFutura", function (value, element) {
        if (value === "") return true;
        var fecha = new Date(value);
        return fecha >= new Date().setHours(0, 0, 0, 0);
    }, "La fecha debe ser hoy o en el futuro.");

    $.validator.addMethod("fechaPosterior", function (value, element, param) {
        if (value === "" || $(param).val() === "") return true;
        var fecha1 = new Date($(param).val());
        var fecha2 = new Date(value);
        return fecha2 > fecha1;
    }, "La fecha debe ser posterior a la fecha de préstamo.");

})();

// Función para actualizar contadores
function actualizarContadores() {
    fetch('/api/estadisticas')
        .then(response => response.json())
        .then(data => {
            document.getElementById('contador-prestamos').textContent = data.prestamosActivos;
            document.getElementById('contador-reservas').textContent = data.reservasActivas;
            document.getElementById('contador-atrasos').textContent = data.prestamosAtrasados;
        })
        .catch(error => console.error('Error:', error));
}

// Ejecutar cuando el documento esté listo
document.addEventListener('DOMContentLoaded', function () {
    // Inicializar componentes
    inicializarComponentes();

    // Actualizar contadores cada 30 segundos
    setInterval(actualizarContadores, 30000);
});

function inicializarComponentes() {
    // Inicializar todos los componentes Bootstrap que necesiten inicialización
    const forms = document.querySelectorAll('.needs-validation');

    forms.forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
}