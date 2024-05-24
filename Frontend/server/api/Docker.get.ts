export default defineEventHandler(async (event) => {
    try {
        let content = await fetch("http://localhost:5173/api/Docker");
        return new Response(JSON.stringify(await content.json()), {
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