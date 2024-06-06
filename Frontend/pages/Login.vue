<template>
    <div class="LoginPage">
        <div class="Title">
            Docker Cloud
        </div>
        <div class="InputCon">
            <input type="text" placeholder="Enter Username" v-model="username">
        </div>
        <div class="InputCon">
            <input type="password" placeholder="Enter Password" v-model="password">
        </div>
        <div class="LoginButton" @click="navigateTo('/')">
            Login
        </div>
        <div class="RegisterButton" @click="navigateTo('/Register')">
            Register
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
.Title{
    font-size: 40pt;
    font-weight: 1000;
    margin-bottom: 40pt;
}
</style>