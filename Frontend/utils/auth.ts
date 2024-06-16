import getCookies from "./getCookies";
export default function checkAuth() {
    let cookies = getCookies();

    if(cookies["username"]){
        return true;
    }else{
        return false;
    }
}