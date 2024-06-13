export default defineEventHandler(async (event) => {
    try {
        try{
            let query = await getQuery(event);
            if(query != null){
                let content = await fetch("http://localhost:5173/api/Docker?name=" + query.name);
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