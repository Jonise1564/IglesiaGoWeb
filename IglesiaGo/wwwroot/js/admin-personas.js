
document.addEventListener("DOMContentLoaded", function () {
    
    // ==========================================
    // 1. EVENT DELEGATION PARA BOTONES DE TABLA
    // ==========================================
    // Escucha clics en la tabla o el documento sin depender de "onclick" inline
    document.addEventListener("click", function (e) {
        
        // A) Botón Cambiar Estado (Activar/Desactivar)
        const btnEstado = e.target.closest(".btn-cambiar-estado");
        if (btnEstado) {
            const id = btnEstado.dataset.id;
            const nombre = btnEstado.dataset.nombre;
            // Convierte el valor string 'true'/'false' a booleano real
            const activar = btnEstado.dataset.activar === "true";
            
            confirmarCambioEstado(id, nombre, activar);
            return;
        }

        // B) Botón Editar Miembro
        const btnEditar = e.target.closest(".btn-editar-miembro");
        if (btnEditar) {
            abrirModalEditarMiembro(btnEditar);
            return;
        }
    });

    // ==========================================
    // 2. SUBMIT DEL FORMULARIO (CREAR / EDITAR)
    // ==========================================
    const form = document.getElementById("formPersonasSection");

    if (form) {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            // Validación de Bootstrap
            if (!form.checkValidity()) {
                e.stopPropagation();
                form.classList.add('was-validated');
                return;
            }

            const idElement = document.getElementById("Id");
            const id = idElement ? parseInt(idElement.value) : 0;

            // Extraemos datos del formulario a JSON
            const formData = new FormData(form);
            const dataObj = {};
            formData.forEach((value, key) => {
                if (key !== "__RequestVerificationToken") {
                    dataObj[key] = value;
                }
            });

            // Ajustes de tipos para C#
            dataObj.FechaNacimiento = (dataObj.FechaNacimiento && dataObj.FechaNacimiento.trim() !== "") ? dataObj.FechaNacimiento : null;
            dataObj.UsuarioId = dataObj.UsuarioId ? dataObj.UsuarioId : null;

            let actionUrl = "/api/personas";
            let httpMethod = "POST";

            if (id > 0) {
                actionUrl = `/api/personas/${id}`;
                httpMethod = "PUT";
            }

            try {
                Swal.fire({
                    title: 'Procesando...',
                    text: 'Guardando los cambios en el sistema.',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                const response = await fetch(actionUrl, {
                    method: httpMethod,
                    body: JSON.stringify(dataObj),
                    headers: {
                        "Content-Type": "application/json"
                    }
                });

                if (response.ok) {
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

// ==========================================
// 3. FUNCIONES GLOBALES DE SOPORTE
// ==========================================

// Cambiar estado (Activar / Desactivar)
window.confirmarCambioEstado = async function (id, nombre, activar) {
    console.log("=== CAMBIO DE ESTADO SOLICITADO ===", { id, nombre, activar });
    
    const accionTexto = activar ? 'activar' : 'desactivar';
    const accionPasado = activar ? 'activado' : 'desactivado';
    const colorBoton = activar ? '#198754' : '#ffc107';

    const resultado = await Swal.fire({
        title: `¿Querés ${accionTexto} a ${nombre}?`,
        text: `El miembro cambiará su estado a ${accionPasado}.`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: colorBoton,
        cancelButtonColor: '#6c757d',
        confirmButtonText: `Sí, ${accionTexto}`,
        cancelButtonText: 'Cancelar'
    });

    if (!resultado.isConfirmed) return;

    try {
        const response = await fetch(`/api/personas/${id}/estado?activar=${activar}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();

        if (response.ok) {
            await Swal.fire({
                title: '¡Completado!',
                text: data.mensaje || `El miembro fue ${accionPasado} correctamente.`,
                icon: 'success',
                timer: 1500,
                showConfirmButton: false
            });
            
            window.location.reload(); 
        } else {
            Swal.fire({
                title: 'No se pudo cambiar el estado',
                text: data.mensaje || 'Ocurrió un error en el servidor.',
                icon: 'error'
            });
        }

    } catch (error) {
        console.error('Error en la petición AJAX:', error);
        Swal.fire({
            title: 'Error de Red',
            text: 'No se pudo comunicar con el servidor.',
            icon: 'warning'
        });
    }
};

// Abrir Modal para Nuevo Miembro
window.abrirModalNuevoMiembro = function () {
    const form = document.getElementById("formPersonasSection");
    if (form) {
        form.reset();
        form.classList.remove('was-validated');
        const idElem = document.getElementById("Id");
        if (idElem) idElem.value = "0";
    }
    
    const modalElement = document.getElementById('modalPersona') || document.getElementById('modalNuevoMiembro');
    if (modalElement) {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    }
};

// Abrir Modal para Editar Miembro
window.abrirModalEditarMiembro = function (btn) {
    const dataset = btn.dataset;
    
    const p = {
        Id: dataset.id,
        Nombres: dataset.nombres,
        Apellidos: dataset.apellidos,
        DocumentoIdentidad: dataset.dni,
        Telefono: dataset.telefono,
        Email: dataset.email,
        Ciudad: dataset.ciudad,
        TipoPersona: dataset.tipo,
        FechaNacimiento: dataset.nacimiento,
        Pais: dataset.pais
    };

    cargarMiembroEnFormulario(p);

    const modalElement = document.getElementById('modalPersona') || document.getElementById('modalNuevoMiembro');
    if (modalElement) {
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    }
};

// Mapeo de datos al Formulario
window.cargarMiembroEnFormulario = function (p) {
    if (!p) return;

    const setVal = (id, val) => {
        const el = document.getElementById(id);
        if (el) el.value = val || "";
    };

    setVal("Id", p.Id || 0);
    setVal("Activo", p.Activo !== undefined ? p.Activo : true);
    setVal("DocumentoIdentidad", p.DocumentoIdentidad);
    setVal("Nombres", p.Nombres);
    setVal("Apellidos", p.Apellidos);
    setVal("Genero", p.Genero);
    setVal("EstadoCivil", p.EstadoCivil);

    const fechaInput = document.getElementById("FechaNacimiento");
    if (fechaInput) {
        if (p.FechaNacimiento) {
            const fecha = new Date(p.FechaNacimiento);
            if (!isNaN(fecha.getTime())) {
                const yyyy = fecha.getFullYear();
                const mm = String(fecha.getMonth() + 1).padStart(2, '0');
                const dd = String(fecha.getDate()).padStart(2, '0');
                fechaInput.value = `${yyyy}-${mm}-${dd}`;
            } else {
                fechaInput.value = p.FechaNacimiento;
            }
        } else {
            fechaInput.value = "";
        }
    }

    setVal("Email", p.Email);
    setVal("Telefono", p.Telefono);
    setVal("TelefonoAlternativo", p.TelefonoAlternativo);
    setVal("Direccion", p.Direccion);
    setVal("Ciudad", p.Ciudad);
    setVal("EstadoProvincia", p.EstadoProvincia);
    setVal("CodigoPostal", p.CodigoPostal);
    setVal("Pais", p.Pais || "Argentina");
    setVal("TipoPersona", p.TipoPersona || "Miembro");
    setVal("UsuarioId", p.UsuarioId);
};