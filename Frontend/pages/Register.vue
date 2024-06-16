<template>
    <div class="LoginPage">
        <div class="InputCon">
            <input type="text" placeholder="Enter Username" v-model="username">
        </div>
        <div class="InputCon">
            <input type="password" placeholder="Enter Password" v-model="password">
        </div>
        <div class="InputCon">
            <input type="password" placeholder="Repeat Password" v-model="repeatPassword">
        </div>
        <div class="RegisterButton" @click=checkRegister()>
            Register
        </div>
        <div v-if="showRegisterError" class="RegisterError">
            Password does not match!
        </div>
    </div>
</template>
<style>
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
.RegisterError{
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
            repeatPassword:"",
            showRegisterError: false
        }
    },
    methods: {
        checkRegister() {
            if (this.username != "" && this.password != "" && this.repeatPassword == this.password) {
                this.register()
            }
            else{
                this.showRegisterError = true
            }
        },
        register(){
            $fetch("/api/User", {
                method: "POST",
                body: {
                    name:this.username,
                    password:this.password
                }
            }).then(res => {
                if (res.message != undefined && res.message == "Accepted!") {
                    navigateTo("/Login")
                } else {
                    this.showRegisterError = true
                    console.log(res)
                }
            }).catch(e => {
                this.showRegisterError = true
                console.log(e)
            })
        }
    }
} 
</script>