<template>
    <div class="LoginPage">
        <div class="Title">
            Docker Cloud
        </div>
        <div class="InputCon">
            <input type="text" placeholder="Enter Username" v-model="username" @keydown.enter="checkLogin()">
        </div>
        <div class="InputCon">
            <input type="password" placeholder="Enter Password" v-model="password" @keydown.enter="checkLogin()">
        </div>
        <div class="LoginButton" @click=checkLogin()>
            Login
        </div>
        <div class="RegisterButton" @click="navigateTo('/Register')">
            Register
        </div>
        <div v-if="showLoginError" class="LoginError">
            Username or Password incorrect!
        </div>
    </div>
</template>

<style scoped>
.LoginPage {
    position: fixed;
    width: 400pt;
    height: 500pt;
    top: calc(50vh - 200pt);
    left: calc(50vw - 200pt);
    background-color: rgb(35, 35, 45);
    border-radius: 25pt;
    text-align: center;
    align-content: center;
}

.InputCon {
    width: 100%;
}

.LoginButton {
    width: 194pt;
    height: 34pt;
    border-radius: 20pt;
    border: solid 3pt rgb(155, 255, 170);
    background-color: transparent;
    font-family: inherit;
    font-size: inherit;
    color: inherit;
    font-weight: inherit;
    margin-top: 40pt;
    margin-left: calc(50% - 100pt);
    text-align: center;
    align-content: center;
}

.LoginButton:hover {
    background-color: rgb(155, 255, 170);
    color: rgb(35, 35, 45);
}

input {
    width: 194pt;
    height: 34pt;
    border-radius: 20pt;
    border: solid 3pt rgb(155, 255, 170);
    background-color: transparent;
    font-family: inherit;
    font-size: inherit;
    color: inherit;
    font-weight: inherit;
    margin-top: 20pt;
    padding-left: 10pt;
    padding-right: 10pt;
}

.RegisterButton {
    width: 194pt;
    height: 34pt;
    border-radius: 20pt;
    border: solid 3pt rgb(155, 255, 170);
    background-color: transparent;
    font-family: inherit;
    font-size: inherit;
    color: inherit;
    font-weight: inherit;
    margin-top: 60pt;
    margin-left: calc(50% - 100pt);
    text-align: center;
    align-content: center;
}

.RegisterButton:hover {
    background-color: rgb(155, 255, 170);
    color: rgb(35, 35, 45);
}

.Title {
    font-size: 40pt;
    font-weight: 1000;
    margin-bottom: 40pt;
}

.LoginError {
    color: rgb(255, 70, 70);
    margin-top: 25pt;
}
</style>
<script>
export default {
    data() {
        return {
            username: "",
            password: "",
            showLoginError: false
        }
    },
    methods: {
        checkLogin() {
            if (this.username != "" && this.password != "") {
                this.login()
            }
            else {
                this.showLoginError = true
            }
        },
        login(){
            $fetch("/api/Login", {
                method: "POST",
                body: {
                    name:this.username,
                    password:this.password
                }
            }).then(res => {
                if (res.ok != undefined && res.ok == true) {
                    document.cookie = "username=" + this.username
                    navigateTo("/")
                } else {
                    this.showLoginError = true
                    console.log(res)
                }
            }).catch(e => {
                this.showLoginError = true
                console.log(e)
            })
        }
    }
} 
</script>