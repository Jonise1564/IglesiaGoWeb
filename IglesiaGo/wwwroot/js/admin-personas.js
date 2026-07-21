
/**
 * admin-personas.js
 * Control de formulario modular y asíncrono conectado al API de Personas en IglesiaGo.
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

            // 3. Extraemos el ID para saber si es una Creación o una Edición
            const idElement = document.getElementById("Id");
            const id = idElement ? parseInt(idElement.value) : 0;

            // 4. Mapeamos los datos del formulario a un Objeto JSON plano (compatible con tu PersonaUpsertDto)
            const formData = new FormData(form);
            const dataObj = {};
            formData.forEach((value, key) => {
                // Saltamos el token antiforgery ya que las APIs REST por defecto no lo procesan igual
                if (key !== "__RequestVerificationToken") {
                    dataObj[key] = value;
                }
            });

            // Ajustes de tipos obligatorios para que el mapeo en C# no falle
            dataObj.FechaNacimiento = (dataObj.FechaNacimiento && dataObj.FechaNacimiento.trim() !== "") ? dataObj.FechaNacimiento : null;
            dataObj.UsuarioId = dataObj.UsuarioId ? dataObj.UsuarioId : null;

            // 5. Configuración dinámica de URL y Método HTTP según corresponda (REST)
            let actionUrl = "/api/personas";
            let httpMethod = "POST"; // Por defecto, Crear

            if (id > 0) {
                actionUrl = `/api/personas/${id}`;
                httpMethod = "PUT"; // Si tiene ID, es Editar
            }

            try {
                // Bloqueo estético de pantalla
                Swal.fire({
                    title: 'Procesando...',
                    text: 'Guardando los cambios en el sistema.',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                // 6. Envío de la petición en formato JSON al API
                const response = await fetch(actionUrl, {
                    method: httpMethod,
                    body: JSON.stringify(dataObj),
                    headers: {
                        "Content-Type": "application/json"
                    }
                });

                // 7. Procesamiento de respuestas REST
                if (response.ok) {
                    // Si es PUT (NoContent / 204), no trae cuerpo JSON
                    let mensajeExito = id > 0 ? 'Miembro actualizado con éxito.' : 'Miembro creado con éxito.';
                    
                    if (httpMethod === "POST") {
                        const resultado = await response.json();
                        mensajeExito = `Miembro ${resultado.nombres} guardado correctamente.`;
                    }

                    Swal.fire({
                        title: '¡Completado!',
                        text: mensajeExito,
                        icon: 'success',
                        confirmButtonText: 'Aceptar',
                        confirmButtonColor: '#0d6efd'
                    }).then(() => {
                        window.location.reload();
                    });
                } else {
                    // Captura errores de validación del API (ej: 400 BadRequest - DNI Duplicado)
                    const errorJson = await response.json();
                    Swal.fire({
                        title: 'No se pudo guardar',
                        text: errorJson.mensaje || 'Verifique los datos ingresados en el formulario.',
                        icon: 'error',
                        confirmButtonText: 'Corregir',
                        confirmButtonColor: '#dc3545'
                    });
                }

            } catch (error) {
                console.error("Error crítico en AJAX hacia el API:", error);
                Swal.fire({
                    title: 'Error de Red',
                    text: 'No se pudo conectar con el servidor API. Reintente en unos instantes.',
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
 * sobre los inputs del Modal.
 * @param {Object} p - Entidad Persona serializada.
 */
function cargarMiembroEnFormulario(p) {
    if (!p) return;

    // Campos ocultos y estructurales
    document.getElementById("Id").value = p.Id || 0;
    
    // Si tu formulario maneja el estado activo
    const activoElement = document.getElementById("Activo");
    if (activoElement) {
        activoElement.value = p.Activo !== undefined ? p.Activo : true;
    }

    // Sección: Datos Personales
    document.getElementById("DocumentoIdentidad").value = p.DocumentoIdentidad || "";
    document.getElementById("Nombres").value = p.Nombres || "";
    document.getElementById("Apellidos").value = p.Apellidos || "";
    
    const generoElement = document.getElementById("Genero");
    if (generoElement) generoElement.value = p.Genero || "";
    
    const estadoCivilElement = document.getElementById("EstadoCivil");
    if (estadoCivilElement) estadoCivilElement.value = p.EstadoCivil || "";

    // Manejo de la fecha (Format: YYYY-MM-DD)
    const fechaInput = document.getElementById("FechaNacimiento");
    if (fechaInput) {
        if (p.FechaNacimiento) {
            const fecha = new Date(p.FechaNacimiento);
            const yyyy = fecha.getFullYear();
            const mm = String(fecha.getMonth() + 1).padStart(2, '0');
            const dd = String(fecha.getDate()).padStart(2, '0');
            fechaInput.value = `${yyyy}-${mm}-${dd}`;
        } else {
            fechaInput.value = "";
        }
    }

    // Sección: Contacto y Ubicación
    document.getElementById("Email").value = p.Email || "";
    document.getElementById("Telefono").value = p.Telefono || "";
    
    const telAltElement = document.getElementById("TelefonoAlternativo");
    if (telAltElement) telAltElement.value = p.TelefonoAlternativo || "";
    
    const dirElement = document.getElementById("Direccion");
    if (dirElement) dirElement.value = p.Direccion || "";
    
    document.getElementById("Ciudad").value = p.Ciudad || "";
    
    const estElement = document.getElementById("EstadoProvincia");
    if (estElement) estElement.value = p.EstadoProvincia || "";
    
    const cpElement = document.getElementById("CodigoPostal");
    if (cpElement) cpElement.value = p.CodigoPostal || "";
    
    document.getElementById("Pais").value = p.Pais || "Argentina";
    document.getElementById("TipoPersona").value = p.TipoPersona || "Miembro";
    
    const userElement = document.getElementById("UsuarioId");
    if (userElement) userElement.value = p.UsuarioId || "";

    // Corrección del scroll innecesario para Modales
    document.getElementById("seccionFormulario")?.scrollIntoView({ behavior: 'smooth' });
}

/**
 * Realiza el borrado lógico (desactivar/activar) de un miembro mediante el API.
 * @param {number} id - ID de la persona.
 * @param {string} nombre - Nombre del miembro para mostrar en los textos.
 * @param {boolean} activar - true si se quiere activar, false si se quiere desactivar.
 */



async function confirmarCambioEstado(id, nombre, activar) {
    const accionTexto = activar ? 'activar' : 'desactivar';
    const accionPasado = activar ? 'activado' : 'desactivado';
    const colorBoton = activar ? '#198754' : '#ffc107';

    // 1. Mostrar cartel de confirmación al usuario
    const resultado = await Swal.fire({
        title: `¿Querés ${accionTexto} a ${nombre}?`,
        text: `El miembro cambiará su estado a ${accionPasado} en el sistema.`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: colorBoton,
        cancelButtonColor: '#6c757d',
        confirmButtonText: `Sí, ${accionTexto}`,
        cancelButtonText: 'Cancelar'
    });

    if (!resultado.isConfirmed) return;

    try {
        // 2. Consumir el endpoint PATCH de la API REST (Correctamente cerrado)
        const response = await fetch(`/api/personas/${id}/estado?activar=${activar}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            // 3. Éxito: Mostrar alerta y recargar
            await Swal.fire({
                title: '¡Operación exitosa!',
                text: data.mensaje || `El miembro fue ${accionPasado} correctamente.`,
                icon: 'success',
                timer: 2000,
                showConfirmButton: false
            });
            
            location.reload(); 
        } else {
            Swal.fire({
                title: 'Error',
                text: data.mensaje || 'No se pudo procesar la solicitud.',
                icon: 'error'
            });
        }

    } catch (error) {
        console.error('Error en la petición:', error);
        Swal.fire({
            title: 'Error de comunicación',
            text: 'No se pudo conectar con el servidor. Intentá de nuevo más tarde.',
            icon: 'error'
        });
    }
}