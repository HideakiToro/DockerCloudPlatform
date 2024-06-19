export default defineEventHandler(async (event) => {
    let body: JSON = await readBody(event);
    try {
        let res = await $fetch("http://localhost:5173/api/User", {
            method: "DELETE",
            body: body
        }).catch(e => {
            console.log(e);
            return new Response(JSON.stringify(e), {
                status: 500,
                headers: { 'Content-Type': 'application/json' }
            });
        });
        return new Response(JSON.stringify(res), {
            status: 200,
            headers: { 'Content-Type': 'application/json' }
        });
    } catch (e) {
        console.log(e);
        return new Response(JSON.stringify({ error: e }), {
            status: 503,
            headers: { 'Content-Type': 'application/json' },
        });
    }
})