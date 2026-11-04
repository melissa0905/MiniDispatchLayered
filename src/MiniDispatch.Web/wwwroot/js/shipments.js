(() => {
    "use strict";

    const table = document.getElementById("shipments-table");
    if (!table) return;

    const statusMessage = document.getElementById("shipments-live-status");
    const tableUrl = new URL(table.dataset.tableUrl, window.location.origin);
    for (const [key, value] of new URLSearchParams(window.location.search)) {
        if (key.toLowerCase() === "status") tableUrl.searchParams.set("status", value);
    }

    if (!window.signalR) {
        statusMessage.textContent = "Canlı güncelleme yüklenemedi. Sayfayı yenileyebilirsiniz.";
        return;
    }

    let refreshPending = false;
    let refreshing = false;
    let refreshTimer;

    // Serialize requests so an older response cannot overwrite newer data.
    async function refreshTable() {
        refreshPending = true;
        if (refreshing) return;
        refreshing = true;
        clearTimeout(refreshTimer);

        try {
            while (refreshPending) {
                refreshPending = false;
                const response = await fetch(tableUrl, {
                    cache: "no-store",
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });
                if (!response.ok) throw new Error(`Table request failed: ${response.status}`);
                table.innerHTML = await response.text();
            }
            statusMessage.textContent = "Canlı güncelleme etkin.";
        } catch (error) {
            console.error("Sevkiyat listesi güncellenemedi.", error);
            statusMessage.textContent = "Liste güncellenemedi. Yeniden deneniyor…";
            refreshTimer = setTimeout(refreshTable, 5000);
        } finally {
            refreshing = false;
        }
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(table.dataset.hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.on("ShipmentsChanged", refreshTable);
    connection.onreconnecting(() => {
        statusMessage.textContent = "Canlı bağlantı yeniden kuruluyor…";
    });
    connection.onreconnected(refreshTable);
    connection.onclose(() => {
        statusMessage.textContent = "Canlı bağlantı kesildi. Yeniden bağlanılıyor…";
        setTimeout(start, 5000);
    });

    async function start() {
        try {
            await connection.start();
        } catch (error) {
            console.error("Canlı bağlantı kurulamadı.", error);
            statusMessage.textContent = "Canlı bağlantı kurulamadı. Yeniden deneniyor…";
            setTimeout(start, 5000);
            return;
        }
        // Include changes made between rendering the page and connecting.
        await refreshTable();
    }

    start();
})();
