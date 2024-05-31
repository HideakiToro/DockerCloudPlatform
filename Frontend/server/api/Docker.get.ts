export default defineEventHandler(async (event) => {
    try {
        try{
            let body = await readBody(event);
            if(body != null){
                let content = await fetch("http://localhost:5173/api/Docker", {
                    body: body
                });
                return new Response(JSON.stringify(await content.json()), {
                    status: 200,
                    headers: { 'Content-Type': 'application/json' },
                });
            } else {
                let content = await fetch("http://localhost:5173/api/Docker");
                return new Response(JSON.stringify(await content.json()), {
                    status: 200,
                    headers: { 'Content-Type': 'application/json' },
                });
            }
        } catch(e) {
            let content = await fetch("http://localhost:5173/api/Docker");
            return new Response(JSON.stringify(await content.json()), {
                status: 200,
                headers: { 'Content-Type': 'application/json' },
            });
        }
    } catch (e) {
        return new Response(JSON.stringify({ error: e }), {
            status: 503,
            headers: { 'Content-Type': 'application/json' },
        });
    }
})