<template>
    <NavBar>Container Info</NavBar>
    <div class="StatusContainer">
        <div class="StatusName">The Container Name</div>
        <div class="StatusType">Intern</div>
        <div class="StatusStatus"></div>
        <div class="StatusLogs">
            <div v-for="(item, index) in 59">
                <div v-if="Math.random() > 0.2">[05/05/2024 01:25:{{ item }}] Server 1: No Issues detected.</div>
                <div v-else>[05/05/2024 01:25:{{ item }}] Server 1: Some Issues detected that might require human intervention.</div>
            </div>
        </div>
        <div class="StatusInfo">
            <div class="StatusInfoTitle">Connection</div>
            IP: 127.0.0.1:4000<br>
            URL: 127.0.0.1:3000/pods?id=15
        </div>
    </div>
</template>

<style>
.StatusContainer {
    position: relative;
    margin: auto;
    background: radial-gradient(closest-side, rgb(65, 65, 75), rgb(35, 35, 45));
    min-width: 200pt;
    width: calc(50vw - 50pt);
    height: calc(100vh - 130pt);
    margin-top: 15pt;
    border-radius: 15pt;
    padding: 25pt;
    justify-content: center;
}

.StatusName {
    text-align: center;
    font-size: 25pt;
    font-weight: 600;
    overflow: auto;
    white-space: nowrap;
}

.StatusType {
    text-align: center;
    margin-top: 10pt;
}

.StatusStatus {
    position: absolute;
    top: 15pt;
    right: 15pt;
    width: 50pt;
    height: 50pt;
    background-color: rgb(70, 255, 110);
    border-radius: 25pt;
}

.StatusLogs {
    line-height: 25pt;
    position: absolute;
    margin: auto;
    border: solid 2pt rgb(155, 255, 170);
    border-radius: 15pt;
    padding: 15pt;
    top: 100pt;
    bottom: 30%;
    left: 12.5%;
    right: 12.5%;
    background-color: rgb(35, 35, 35);
    color: white;
    overflow: auto;
}

.StatusLogs::-webkit-scrollbar {
    background: transparent;
}

.StatusLogs::-webkit-scrollbar-thumb {
    background: rgb(155, 255, 170);
    border-radius: 5pt;
}

.StatusInfo {
    position: absolute;
    text-align: center;
    border: solid 2pt rgb(155, 255, 170);
    border-radius: 15pt;
    padding: 15pt;
    margin: auto;
    bottom: 25pt;
    left: 12.5%;
    right: 12.5%;
    height: 12.5%;
    overflow: auto;
}

.StatusInfo::-webkit-scrollbar {
    background: transparent;
}

.StatusInfo::-webkit-scrollbar-thumb {
    background: rgb(155, 255, 170);
    border-radius: 5pt;
}

.StatusInfoTitle {
    margin-bottom: 15pt;
    text-decoration: underline;
}
</style>