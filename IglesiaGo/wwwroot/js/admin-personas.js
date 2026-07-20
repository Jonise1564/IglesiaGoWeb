/**
 * admin-personas.js
 * Control de formulario modular y asíncrono para la sección de Personas en IglesiaGo.
 */

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("formPersonasSection");

    if (form) {
        form.addEventListener("submit", async function (e) {
            // 1. Evita la recarga completa de la página
            e.preventDefault();

            // 2. Validación nativa de Bootstrap
            if (!form.checkValidity()) {
                e.stopPropagation();
                form.classList.add('was-validated');
                return;
            }

            // 3. Empaquetado automático de inputs
            const formData = new FormData(form);
            const actionUrl = form.getAttribute("action");

            try {
                // Bloqueo de pantalla estético mientras procesa la base de datos
                Swal.fire({
                    title: 'Procesando...',
                    text: 'Guardando los cambios en el sistema.',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                // 4. Envío por Fetch a tu PersonasController
                const response = await fetch(actionUrl, {
                    method: "POST",
                    body: formData,
                    headers: {
                        // Token de seguridad crucial para evitar errores HTTP 400/419 en ASP.NET Core
                        "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]')?.value
                    }
                });

                const resultado = await response.json();

                if (response.ok && resultado.success) {
                    Swal.fire({
                        title: '¡Completado!',
                        text: resultado.mensaje || 'Miembro guardado con éxito.',
                        icon: 'success',
                        confirmButtonText: 'Aceptar',
                        confirmButtonColor: '#0d6efd'
                    }).then(() => {
                        // Refresca el Dashboard para actualizar la tabla y las tarjetas de contadores
                        window.location.reload();
                    });
                } else {
                    // Manejo de errores lógicos del backend (ej: DNI repetido)
                    Swal.fire({
                        title: 'No se pudo guardar',
                        text: resultado.mensaje || 'Verifique los datos ingresados.',
                        icon: 'error',
                        confirmButtonText: 'Corregir',
                        confirmButtonColor: '#dc3545'
                    });
                }

            } catch (error) {
                console.error("Error crítico en AJAX:", error);
                Swal.fire({
                    title: 'Error de Red',
                    text: 'No se pudo conectar con el servidor. Reintente en unos instantes.',
                    icon: 'warning',
                    confirmButtonText: 'Entendido',
                    confirmButtonColor: '#ffc107'
                });
            }
        });
    }
});

/**
 * Mapea los datos del modelo Persona seleccionados en la tabla directamente
 * sobre los inputs generados por los Tag Helpers de la vista parcial.
 * @param {Object} p - Entidad Persona serializada desde Razor.
 */
function cargarMiembroEnFormulario(p) {
    if (!p) return;

    // Campos ocultos y estructurales
    document.getElementById("Id").value = p.Id || 0;
    document.getElementById("Activo").value = p.Activo !== undefined ? p.Activo : true;

    // Sección: Datos Personales
    document.getElementById("DocumentoIdentidad").value = p.DocumentoIdentidad || "";
    document.getElementById("Nombres").value = p.Nombres || "";
    document.getElementById("Apellidos").value = p.Apellidos || "";
    document.getElementById("Genero").value = p.Genero || "";
    document.getElementById("EstadoCivil").value = p.EstadoCivil || "";

    // Manejo correcto de la fecha para input type="date" (Formato requerido: YYYY-MM-DD)
    if (p.FechaNacimiento) {
        const fecha = new Date(p.FechaNacimiento);
        const yyyy = fecha.getFullYear();
        const mm = String(fecha.getMonth() + 1).padStart(2, '0');
        const dd = String(fecha.getDate()).padStart(2, '0');
        document.getElementById("FechaNacimiento").value = `${yyyy}-${mm}-${dd}`;
    } else {
        document.getElementById("FechaNacimiento").value = "";
    }

    // Sección: Contacto y Ubicación
    document.getElementById("Email").value = p.Email || "";
    document.getElementById("Telefono").value = p.Telefono || "";
    document.getElementById("TelefonoAlternativo").value = p.TelefonoAlternativo || "";
    document.getElementById("Direccion").value = p.Direccion || "";
    document.getElementById("Ciudad").value = p.Ciudad || "";
    document.getElementById("EstadoProvincia").value = p.EstadoProvincia || "";
    document.getElementById("CodigoPostal").value = p.CodigoPostal || "";
    document.getElementById("Pais").value = p.Pais || "Argentina";
    document.getElementById("TipoPersona").value = p.TipoPersona || "Miembro";
    document.getElementById("UsuarioId").value = p.UsuarioId || "";

    // 5. Cambio dinámico de interfaz para indicar modo Edición
    const cardTitle = document.querySelector("#seccionFormulario h2");
    if (cardTitle) {
        cardTitle.innerHTML = '<i class="fa-solid fa-user-pen me-2"></i>Editar Miembro';
    }

    // Hace scroll suave hacia el formulario si estás en pantallas chicas
    document.getElementById("seccionFormulario").scrollIntoView({ behavior: 'smooth' });
}