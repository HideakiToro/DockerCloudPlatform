export default defineEventHandler(async (event) => {
    let body: JSON = await readBody(event);
    try {
        let content = await $fetch("http://localhost:5173/api/Docker", {
            method: "DELETE",
            body: body
        }).catch(e => {
            return new Response(JSON.stringify(e), {
                status: 500,
                headers: { 'Content-Type': 'application/json' }
            });
        });
        return new Response(JSON.stringify(content), {
            status: 200,
            headers: { 'Content-Type': 'application/json' },
        });
    } catch (e) {
        return new Response(JSON.stringify({ error: e }), {
            status: 503,
            headers: { 'Content-Type': 'application/json' },
        });
    }
})