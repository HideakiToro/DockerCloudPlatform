export default function getCookies() {
    const allCookies = document.cookie;

    // Parse cookies into an object
    let cookies = Object.fromEntries(allCookies.split('; ').map(c => {
      const [key, ...v] = c.split('=');
      return [key, v.join('=')];
    }));
    
    return cookies;
}