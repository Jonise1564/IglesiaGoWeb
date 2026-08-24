// /**
//  * admin-mensajes.js
//  * Gestión de Consultas y Mensajes del sistema IglesiaGo
//  */

// let bootstrapMensajeModal = null;

// document.addEventListener("DOMContentLoaded", function () {
//     // Inicializar la instancia del Modal de Bootstrap
//     const modalElem = document.getElementById('mensajeModal');
//     if (modalElem) {
//         bootstrapMensajeModal = new bootstrap.Modal(modalElem);
//     }

//     // Cargar la lista inicial de mensajes
//     cargarMensajes();
// });

// /**
//  * Carga el listado de consultas desde el API
//  */
// function cargarMensajes() {
//     const token = localStorage.getItem("token");

//     fetch('/api/ConsultasApi', {
//         headers: {
//             'Authorization': `Bearer ${token}`,
//             'Content-Type': 'application/json'
//         }
//     })
//     .then(res => {
//         if (res.status === 401) {
//             window.location.href = "/Home/Login";
//             return [];
//         }
//         return res.ok ? res.json() : [];
//     })
//     .then(data => {
//         const tbody = document.getElementById("tablaMensajes");
//         if (!tbody) return;

//         tbody.innerHTML = "";

//         if (!data || data.length === 0) {
//             tbody.innerHTML = `<tr><td colspan="5" class="text-center py-4 text-muted">Buzón de entrada vacío.</td></tr>`;
//             return;
//         }

//         data.forEach(m => {
//             const id = m.id || m.Id;
//             const remitente = m.nombreRemitente || m.NombreRemitente || m.nombre || m.Nombre || 'Anónimo';
//             const correo = m.correo || m.Correo || m.email || m.Email || '';
//             const asunto = m.asunto || m.Asunto || 'Sin asunto';
//             const fechaVal = m.fechaCreacion || m.FechaCreacion || m.fechaEnvio || m.FechaEnvio;
//             const fecha = fechaVal ? new Date(fechaVal).toLocaleDateString() : '-';

//             // Evalúa estado 'atendido' o 'leido'
//             const atendido = m.atendido ?? m.Atendido ?? m.leido ?? m.Leido ?? false;

//             tbody.innerHTML += `
//             <tr>
//                 <td>
//                     <div class="fw-bold">${remitente}</div>
//                     <small class="text-muted">${correo}</small>
//                 </td>
//                 <td>${asunto}</td>
//                 <td>${fecha}</td>
//                 <td>
//                     <span class="badge ${atendido ? 'bg-success' : 'bg-warning text-dark'}">
//                         ${atendido ? '<i class="fa-solid fa-check me-1"></i>Atendido' : '<i class="fa-solid fa-clock me-1"></i>Pendiente'}
//                     </span>
//                 </td>
//                 <td class="text-end">
//                     <button class="btn btn-sm btn-outline-primary border-0 me-1" title="Ver / Responder" onclick="verMensaje(${id})">
//                         <i class="fa-solid fa-envelope-open"></i>
//                     </button>
//                     <button class="btn btn-sm btn-outline-danger border-0" title="Eliminar" onclick="eliminarMensaje(${id})">
//                         <i class="fa-solid fa-trash"></i>
//                     </button>
//                 </td>
//             </tr>
//             `;
//         });
//     })
//     .catch(err => {
//         console.error("Error al obtener consultas:", err);
//         const tbody = document.getElementById("tablaMensajes");
//         if (tbody) {
//             tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger py-3">Error al conectar con la API de Consultas.</td></tr>`;
//         }
//     });
// }

// /**
//  * Obtiene el detalle de una consulta específica y abre el modal
//  * @param {number} id ID de la consulta
//  */
// async function verMensaje(id) {
//     const token = localStorage.getItem("token");
//     try {
//         const res = await fetch(`/api/ConsultasApi/${id}`, {
//             headers: {
//                 'Authorization': `Bearer ${token}`
//             }
//         });

//         if (!res.ok) throw new Error("No se pudo obtener el detalle de la consulta.");

//         const consulta = await res.json();

//         const remitente = consulta.nombreRemitente || consulta.NombreRemitente || consulta.nombre || 'Anónimo';
//         const correo = consulta.correo || consulta.Correo || consulta.email || '';

//         // Poblar campos del modal
//         const remitenteElem = document.getElementById("modalMensajeRemitente");
//         const asuntoElem = document.getElementById("modalMensajeAsunto");
//         const cuerpoElem = document.getElementById("modalMensajeCuerpo");
//         const idElem = document.getElementById("modalConsultaId");
//         const respuestaElem = document.getElementById("modalRespuestaTexto");

//         if (remitenteElem) remitenteElem.textContent = `${remitente} (${correo})`;
//         if (asuntoElem) asuntoElem.textContent = consulta.asunto || consulta.Asunto || 'Sin asunto';
//         if (cuerpoElem) cuerpoElem.textContent = consulta.mensaje || consulta.Mensaje || consulta.contenido || consulta.Contenido || 'Sin contenido';
//         if (idElem) idElem.value = consulta.id || consulta.Id;
//         if (respuestaElem) respuestaElem.value = ""; // Limpiar campo de respuesta anterior

//         // Mostrar el modal
//         if (bootstrapMensajeModal) {
//             bootstrapMensajeModal.show();
//         } else {
//             const modalElem = document.getElementById('mensajeModal');
//             bootstrapMensajeModal = new bootstrap.Modal(modalElem);
//             bootstrapMensajeModal.show();
//         }
//     } catch (err) {
//         console.error(err);
//         Swal.fire({
//             title: 'Error',
//             text: 'No se pudo abrir el detalle de la consulta.',
//             icon: 'error'
//         });
//     }
// }

// /**
//  * Envía la respuesta al backend para registrarla en el hilo y marcar la consulta como atendida
//  */
// async function enviarRespuestaConsulta() {
//     const token = localStorage.getItem("token");
//     const consultaIdInput = document.getElementById("modalConsultaId");
//     const respuestaInput = document.getElementById("modalRespuestaTexto");

//     const consultaId = consultaIdInput ? parseInt(consultaIdInput.value) : 0;
//     const respuestaTexto = respuestaInput ? respuestaInput.value.trim() : "";

//     if (!respuestaTexto) {
//         Swal.fire({
//             title: 'Campo Requerido',
//             text: 'Escribe una respuesta antes de enviar.',
//             icon: 'warning'
//         });
//         return;
//     }

//     const payload = {
//         ConsultaId: consultaId,
//         Mensaje: respuestaTexto,
//         EsDelAdministrador: true
//     };

//     try {
//         const res = await fetch('/api/ConsultasApi/Responder', {
//             method: 'POST',
//             headers: {
//                 'Authorization': `Bearer ${token}`,
//                 'Content-Type': 'application/json'
//             },
//             body: JSON.stringify(payload)
//         });

//         if (res.ok) {
//             Swal.fire({
//                 title: '¡Enviado!',
//                 text: 'Respuesta guardada con éxito y la consulta fue marcada como atendida.',
//                 icon: 'success',
//                 confirmButtonColor: '#0d6efd'
//             });

//             if (respuestaInput) respuestaInput.value = "";
//             if (bootstrapMensajeModal) bootstrapMensajeModal.hide();

//             // Recargar la tabla para ver reflejado el cambio de estado
//             cargarMensajes();
//         } else {
//             const errData = await res.json().catch(() => null);
//             Swal.fire({
//                 title: 'Error',
//                 text: (errData && errData.mensaje) ? errData.mensaje : 'Ocurrió un error al enviar la respuesta.',
//                 icon: 'error'
//             });
//         }
//     } catch (err) {
//         console.error("Error al enviar respuesta:", err);
//         Swal.fire({
//             title: 'Error de Conexión',
//             text: 'No se pudo establecer comunicación con el servidor.',
//             icon: 'error'
//         });
//     }
// }

// /**
//  * Elimina una consulta y todo su historial mediante DELETE
//  * @param {number} id ID de la consulta a borrar
//  */
// function eliminarMensaje(id) {
//     Swal.fire({
//         title: '¿Eliminar consulta?',
//         text: "Se borrará la consulta y todo su historial de mensajes.",
//         icon: 'warning',
//         showCancelButton: true,
//         confirmButtonColor: '#dc3545',
//         cancelButtonColor: '#6c757d',
//         confirmButtonText: 'Sí, eliminar',
//         cancelButtonText: 'Cancelar'
//     }).then(async (result) => {
//         if (result.isConfirmed) {
//             const token = localStorage.getItem("token");
//             try {
//                 const res = await fetch(`/api/ConsultasApi/${id}`, {
//                     method: 'DELETE',
//                     headers: {
//                         'Authorization': `Bearer ${token}`
//                     }
//                 });

//                 if (res.ok) {
//                     Swal.fire({
//                         title: '¡Eliminado!',
//                         text: 'La consulta ha sido borrada.',
//                         icon: 'success',
//                         confirmButtonColor: '#0d6efd'
//                     });
//                     cargarMensajes();
//                 } else {
//                     Swal.fire({
//                         title: 'Error',
//                         text: 'No se pudo eliminar la consulta.',
//                         icon: 'error'
//                     });
//                 }
//             } catch (err) {
//                 console.error("Error al eliminar consulta:", err);
//                 Swal.fire({
//                     title: 'Error de Conexión',
//                     text: 'No se pudo comunicar con el servidor.',
//                     icon: 'error'
//                 });
//             }
//         }
//     });
// }