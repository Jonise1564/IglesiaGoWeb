let modalEnsenanza = null;

document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("modalEnsenanza");
    if (modal) {
        modalEnsenanza = new bootstrap.Modal(modal);
    }
    cargarEnsenanzas();
});

//======================================
// HELPER: CONVERTIR A EMBED YOUTUBE
//======================================

function convertirAEmbedYoutube(url) {
    if (!url) return "";
    url = url.trim();

    // Si ya tiene el formato embed, se retorna tal cual
    if (url.includes("youtube.com/embed/")) {
        return url;
    }

    // RegEx para capturar el ID del vídeo de múltiples formatos de YouTube:
    // - https://www.youtube.com/watch?v=ugtyvRCTNkY
    // - https://youtu.be/ugtyvRCTNkY
    // - https://www.youtube.com/shorts/ugtyvRCTNkY
    // - https://youtube.com/v/ugtyvRCTNkY
    const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=|shorts\/)([^#\&\?]*).*/;
    const match = url.match(regExp);

    if (match && match[2].length === 11) {
        return `https://www.youtube.com/embed/${match[2]}`;
    }

    // Si no coincide con un formato de YouTube válido, se deja el valor original
    return url;
}

//======================================
// LISTAR
//======================================

async function cargarEnsenanzas() {
    const tbody = document.getElementById("tablaEnsenanzas");
    if (!tbody) return;

    tbody.innerHTML = `
        <tr>
            <td colspan="4" class="text-center py-4">
                <div class="spinner-border spinner-border-sm me-2"></div>
                Cargando enseñanzas...
            </td>
        </tr>`;

    try {
        const response = await fetch("/api/EnsenanzasApi");

        if (!response.ok)
            throw new Error(`HTTP ${response.status}`);

        const lista = await response.json();
        tbody.innerHTML = "";

        if (!lista || lista.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="4" class="text-center text-muted">
                        No existen enseñanzas.
                    </td>
                </tr>`;
            return;
        }

        lista.forEach(e => {
            const id = e.id ?? e.Id;
            const titulo = e.titulo ?? e.Titulo ?? "Sin título";
            const pasaje = e.pasajeClave ?? e.PasajeClave;
            const videoUrl = e.videoUrl ?? e.VideoUrl;
            const fechaRaw = e.fechaPublicacion ?? e.FechaPublicacion;

            let fecha = "";
            if (fechaRaw) {
                const parts = fechaRaw.substring(0, 10).split('-');
                fecha = parts.length === 3 ? `${parts[2]}/${parts[1]}/${parts[0]}` : new Date(fechaRaw).toLocaleDateString("es-AR");
            }

            tbody.innerHTML += `
                <tr>
                    <td>
                        <div class="fw-bold">${titulo}</div>
                        ${pasaje ? `<small class="text-muted"><i class="fa-solid fa-book-open me-1"></i>${pasaje}</small>` : ''}
                    </td>
                    <td>${fecha}</td>
                    <td>
                        ${
                            videoUrl && videoUrl.trim() !== ""
                                ? `<a href="${videoUrl}" target="_blank" class="btn btn-sm btn-outline-danger py-0 px-2"><i class="fa-brands fa-youtube me-1"></i>Ver video</a>`
                                : "-"
                        }
                    </td>
                    <td class="text-end">
                        <button class="btn btn-warning btn-sm me-1" onclick="editarEnsenanza(${id})">
                            <i class="fa-solid fa-pen"></i>
                        </button>
                        <button class="btn btn-danger btn-sm" onclick="eliminarEnsenanza(${id})">
                            <i class="fa-solid fa-trash"></i>
                        </button>
                    </td>
                </tr>`;
        });

    } catch (ex) {
        console.error(ex);
        tbody.innerHTML = `
            <tr>
                <td colspan="4" class="text-danger text-center">
                    ${ex.message}
                </td>
            </tr>`;
    }
}

//======================================
// NUEVA
//======================================

function crearNuevaEnsenanza() {
    document.getElementById("tituloModalEnsenanza").innerText = "Nueva Enseñanza";
    document.getElementById("formEnsenanza").reset();
    document.getElementById("Id").value = "";

    document.getElementById("FechaPublicacion").value = new Date().toISOString().substring(0, 10);

    modalEnsenanza.show();
}

//======================================
// EDITAR
//======================================

async function editarEnsenanza(id) {
    try {
        const response = await fetch(`/api/EnsenanzasApi/${id}`);

        if (!response.ok) {
            alert("No se pudo obtener la enseñanza.");
            return;
        }

        const e = await response.json();

        document.getElementById("tituloModalEnsenanza").innerText = "Editar Enseñanza";

        document.getElementById("Id").value = e.id ?? e.Id ?? "";
        document.getElementById("Titulo").value = e.titulo ?? e.Titulo ?? "";
        document.getElementById("Contenido").value = e.contenido ?? e.Contenido ?? "";

        const inputPasaje = document.getElementById("PasajeClave");
        if (inputPasaje) inputPasaje.value = e.pasajeClave ?? e.PasajeClave ?? "";

        const inputVideo = document.getElementById("VideoUrl");
        if (inputVideo) inputVideo.value = e.videoUrl ?? e.VideoUrl ?? "";

        const fecha = e.fechaPublicacion ?? e.FechaPublicacion;
        if (fecha) {
            document.getElementById("FechaPublicacion").value = fecha.substring(0, 10);
        } else {
            document.getElementById("FechaPublicacion").value = "";
        }

        modalEnsenanza.show();
    } catch (ex) {
        console.error("Error al cargar para edición:", ex);
        alert("Ocurrió un error al cargar la enseñanza.");
    }
}

//======================================
// GUARDAR
//======================================

async function guardarEnsenanza() {
    const id = document.getElementById("Id").value;
    const inputPasaje = document.getElementById("PasajeClave");
    const inputVideo = document.getElementById("VideoUrl");

    const pasajeVal = inputPasaje ? inputPasaje.value.trim() : "";
    
    // Formatear la URL a https://www.youtube.com/embed/ID
    const rawVideoVal = inputVideo ? inputVideo.value.trim() : "";
    const videoVal = convertirAEmbedYoutube(rawVideoVal);

    const tituloVal = document.getElementById("Titulo").value.trim();
    const contenidoVal = document.getElementById("Contenido").value.trim();
    const fechaVal = document.getElementById("FechaPublicacion").value;

    const modelo = {
        id: id === "" ? 0 : Number(id),
        Id: id === "" ? 0 : Number(id),
        titulo: tituloVal,
        Titulo: tituloVal,
        contenido: contenidoVal,
        Contenido: contenidoVal,
        pasajeClave: pasajeVal,
        PasajeClave: pasajeVal,
        videoUrl: videoVal,
        VideoUrl: videoVal,
        fechaPublicacion: fechaVal,
        FechaPublicacion: fechaVal
    };

    let response;

    if (modelo.id === 0) {
        response = await fetch("/api/EnsenanzasApi", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(modelo)
        });
    } else {
        response = await fetch(`/api/EnsenanzasApi/${modelo.id}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(modelo)
        });
    }

    if (!response.ok) {
        alert("No se pudo guardar la enseñanza.");
        return;
    }

    modalEnsenanza.hide();
    cargarEnsenanzas();
}

//======================================
// ELIMINAR
//======================================

async function eliminarEnsenanza(id) {
    if (!confirm("¿Eliminar la enseñanza?"))
        return;

    const response = await fetch(`/api/EnsenanzasApi/${id}`, {
        method: "DELETE"
    });

    if (!response.ok) {
        alert("No se pudo eliminar.");
        return;
    }

    cargarEnsenanzas();
}

function filtrarEnsenanzas() {
    const query = document.getElementById('buscarEnsenanzaInput').value.toLowerCase().trim();
    const filas = document.querySelectorAll('#tablaEnsenanzas tr');

    filas.forEach(fila => {
        // Ignorar la fila de "Cargando..." o "Sin resultados"
        if (fila.children.length === 1) return;

        const textoFila = fila.innerText.toLowerCase();
        
        if (textoFila.includes(query)) {
            fila.style.display = '';
        } else {
            fila.style.display = 'none';
        }
    });
}