let modalEnsenanza = null;

document.addEventListener("DOMContentLoaded", () => {

    const modal = document.getElementById("modalEnsenanza");

    if (modal) {
        modalEnsenanza = new bootstrap.Modal(modal);
    }

    cargarEnsenanzas();

});

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

        console.log("Enseñanzas:", lista);

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
            const titulo = e.titulo ?? e.Titulo;
            const contenido = e.contenido ?? e.Contenido;
            const videoUrl = e.videoUrl ?? e.VideoUrl;
            const fechaRaw = e.fechaPublicacion ?? e.FechaPublicacion;

            const fecha = fechaRaw
                ? new Date(fechaRaw).toLocaleDateString("es-AR")
                : "";

            tbody.innerHTML += `
                <tr>

                    <td>${titulo}</td>

                    <td>${fecha}</td>

                    <td>
                        ${
                            videoUrl
                                ? `<a href="${videoUrl}" target="_blank">Ver video</a>`
                                : "-"
                        }
                    </td>

                    <td class="text-end">

                        <button class="btn btn-warning btn-sm me-1"
                                onclick="editarEnsenanza(${id})">
                            <i class="fa-solid fa-pen"></i>
                        </button>

                        <button class="btn btn-danger btn-sm"
                                onclick="eliminarEnsenanza(${id})">
                            <i class="fa-solid fa-trash"></i>
                        </button>

                    </td>

                </tr>`;
        });

    }
    catch (ex) {

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

    document.getElementById("FechaPublicacion").value =
        new Date().toISOString().substring(0,10);

    modalEnsenanza.show();

}

//======================================
// EDITAR
//======================================

async function editarEnsenanza(id) {

    const response = await fetch(`/api/EnsenanzasApi/${id}`);

    if (!response.ok) {

        alert("No se pudo obtener la enseñanza.");

        return;
    }

    const e = await response.json();

    document.getElementById("tituloModalEnsenanza").innerText = "Editar Enseñanza";

    document.getElementById("Id").value = e.id ?? e.Id;
    document.getElementById("Titulo").value = e.titulo ?? e.Titulo;
    document.getElementById("Contenido").value = e.contenido ?? e.Contenido;
    document.getElementById("VideoUrl").value = e.videoUrl ?? e.VideoUrl ?? "";

    const fecha = e.fechaPublicacion ?? e.FechaPublicacion;

    if (fecha)
        document.getElementById("FechaPublicacion").value = fecha.substring(0,10);

    modalEnsenanza.show();

}

//======================================
// GUARDAR
//======================================

async function guardarEnsenanza() {

    const id = document.getElementById("Id").value;

    const modelo = {

        id: id === "" ? 0 : Number(id),

        titulo: document.getElementById("Titulo").value,

        contenido: document.getElementById("Contenido").value,

        videoUrl: document.getElementById("VideoUrl").value,

        fechaPublicacion: document.getElementById("FechaPublicacion").value

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

        alert("No se pudo guardar.");

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