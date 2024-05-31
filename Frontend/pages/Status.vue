<template>
    <NavBar>Container Info</NavBar>
    <div class="StatusContainer">
        <div class="StatusName">{{ name }}</div>
        <div class="StatusType">Intern</div>
        <div v-if="info.status == 'Up'" class="StatusStatus"></div>
        <div v-else-if="info.status == 'Exited'" class="StatusStatus StatusRed"></div>
        <div v-else class="StatusStatus StatusYellow"></div>
        <div class="StatusLogs">
            <div v-for="(item, index) in info.logs">
                <div>{{ item }}</div>
            </div>
        </div>
        <div class="StatusInfo">
            <div class="StatusInfoTitle">Connection</div>
            <div v-if="connectedByPort">IP: 127.0.0.1:{{ info.port }}</div>
            <div v-else>URL: 127.0.0.1:3000/pods?id=15</div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            name: this.$route.query.name,
            connectedByPort: true,
            info: {
                status: "Pending",
                logs: [],
                port: -1
            }
        }
    },
    mounted() {
        $fetch("/api/Docker?name=" + this.name).then(res => {
            this.info = res;
        }).catch(e => {
            this.info.status = "Exited"
            this.info.logs.push(e);
        })
    }
}
</script>

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

.StatusYellow {
    background-color: yellow;
}

.StatusRed {
    background-color: rgb(255,70,70);
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