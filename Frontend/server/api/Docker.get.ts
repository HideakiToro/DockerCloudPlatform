export default defineEventHandler(async (event) => {
    try {
        let cookies = event.headers.get('Cookie')
        let cookieStr = cookies ? cookies : ""
        let query = await getQuery(event);
        if (query != null && query.name != null) {
            let content = await fetch("http://localhost:5173/api/Docker?name=" + query.name, {
                headers: {
                    'Cookie': cookieStr
                }
            });
            return new Response(JSON.stringify(await content.json()), {
                status: 200,
                headers: {
                    'Content-Type': 'application/json'
                },
            });
        } else {
            let content = await fetch("http://localhost:5173/api/Docker", {
                headers: {
                    'Cookie': cookieStr
                }
            });
            return new Response(JSON.stringify(await content.json()), {
                status: 200,
                headers: { 'Content-Type': 'application/json' },
            });
        }
    } catch (e) {
        console.log(e);
        return new Response(JSON.stringify({ error: e }), {
            status: 503,
            headers: { 'Content-Type': 'application/json' },
        });
    }
})