export default defineEventHandler(async (event) => {
    let body: JSON = await readBody(event);
    let cookies = event.headers.get('Cookie')
    let cookieStr = cookies ? cookies : ""
    try {
        let content = await $fetch("http://localhost:5173/api/Docker", {
            method: "DELETE",
            body: body,
            headers: {
                "Cookie": cookieStr
            }
        }).catch(e => {
            console.log(e);
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
        console.log(e);
        return new Response(JSON.stringify({ error: e }), {
            status: 503,
            headers: { 'Content-Type': 'application/json' },
        });
    }
})