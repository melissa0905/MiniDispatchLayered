const { test } = require('node:test');
const assert = require('node:assert/strict');
const vm = require('node:vm');
const fs = require('node:fs');
const path = require('node:path');
const script = fs.readFileSync(path.join(__dirname, '../src/MiniDispatch.Web/wwwroot/js/shipments.js'), 'utf8');
const settle = () => new Promise(resolve => setImmediate(resolve));

function tab(search = '') {
    const table = { dataset: { tableUrl: '/Shipments/Table', hubUrl: '/hubs/shipments' }, innerHTML: 'old' };
    const status = { textContent: '' };
    const requests = [];
    const timers = [];
    let listener, reconnected, closed;
    const connection = {
        on(name, callback) { assert.equal(name, 'ShipmentsChanged'); listener = callback; },
        onreconnecting() {},
        onreconnected(callback) { reconnected = callback; },
        onclose(callback) { closed = callback; },
        async start() {}
    };
    class HubConnectionBuilder {
        withUrl(url) { assert.equal(url, '/hubs/shipments'); return this; }
        withAutomaticReconnect() { return this; }
        build() { return connection; }
    }
    const context = {
        document: { getElementById: id => id === 'shipments-table' ? table : status },
        window: { location: { origin: 'http://localhost', search }, signalR: {} },
        signalR: { HubConnectionBuilder }, URL, URLSearchParams,
        console: { error() {} },
        setTimeout(callback) { timers.push(callback); return timers.length; }, clearTimeout() {},
        fetch(url, options) {
            return new Promise(resolve => requests.push({ url: String(url), options, resolve }));
        }
    };
    vm.runInNewContext(script, context);
    return { table, status, requests, timers, changed: () => listener(), reconnect: () => reconnected(), close: () => closed() };
}
function respond(request, html, ok = true) {
    request.resolve({ ok, status: ok ? 200 : 500, text: async () => html });
}

test('open tabs refresh independently and keep their current filters', async () => {
    const all = tab();
    const filtered = tab('?Status=Created');
    await settle();
    assert.equal(all.requests[0].url, 'http://localhost/Shipments/Table');
    assert.equal(filtered.requests[0].url, 'http://localhost/Shipments/Table?status=Created');
    respond(all.requests[0], 'initial'); respond(filtered.requests[0], 'initial');
    await settle();
    const refreshes = [all.changed(), filtered.changed()];
    respond(all.requests[1], 'updated shipment'); respond(filtered.requests[1], 'no matching shipments');
    await Promise.all(refreshes);
    assert.equal(all.table.innerHTML, 'updated shipment');
    assert.equal(filtered.table.innerHTML, 'no matching shipments');
    assert.equal(filtered.requests[1].options.cache, 'no-store');
});

test('changes during a fetch queue another refresh; reconnect also refreshes', async () => {
    const page = tab('?status=InTransit');
    await settle();
    page.changed(); page.changed();
    assert.equal(page.requests.length, 1);
    respond(page.requests[0], 'older');
    await settle();
    assert.equal(page.requests.length, 2);
    respond(page.requests[1], 'newest');
    await settle();
    assert.equal(page.table.innerHTML, 'newest');
    const reconnect = page.reconnect();
    respond(page.requests[2], 'after reconnect');
    await reconnect;
    assert.equal(page.table.innerHTML, 'after reconnect');
});

test('failed fetch preserves table and retries', async () => {
    const page = tab();
    await settle();
    respond(page.requests[0], '', false);
    await settle();
    assert.equal(page.table.innerHTML, 'old');
    assert.equal(page.timers.length, 1);
    const retry = page.timers[0]();
    respond(page.requests[1], 'recovered');
    await retry;
    assert.equal(page.table.innerHTML, 'recovered');
    assert.equal(page.status.textContent, 'Canlı güncelleme etkin.');
});
