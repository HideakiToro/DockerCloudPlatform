<template>
    <NavBar>
        Container Info
        <div class="removeCont" @click="toggleRemoveContainer(true)">
            -
        </div>
    </NavBar>
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
            <div v-if="connectedByPort">IP: {{ host }}:{{ info.port }}</div>
            <div v-else>URL: {{ host }}]:3000/pods?id=-1</div>
        </div>

    </div>
    <div class="removeBackground" v-if="showRemoveCont">
        <div class="removeContWindow">
            <div v-if="!isLoading">
                <div>
                    Do you really want to delete the Container?
                </div>
                <button class="cancelRemBtn" @click="toggleRemoveContainer(false)">
                    Cancel
                </button>
                <button class="confirmRemBtn" @click="console.log('Removed Container'); removeCont()">
                    Remove
                </button>
            </div>
            <div v-else>
                Container is being deleted...
            </div>
        </div>
    </div>
</template>

<script>
import checkAuth from '~/utils/auth';
import getCookies from '~/utils/getCookies'

export default {
    data() {
        return {
            showRemoveCont: false,
            isLoading: false,
            name: this.$route.query.name,
            connectedByPort: true,
            info: {
                status: "Pending",
                logs: [],
                port: -1
            },
            cookies: null,
            host: window.location.hostname
        }
    },
    mounted() {
        if(!checkAuth()) navigateTo("/Login")

        this.cookies = getCookies();

        $fetch("/api/Docker?name=" + this.name).then(res => {
            this.info = res;
        }).catch(e => {
            this.info.status = "Exited"
            this.info.logs.push(e);
        })
    },
    methods: {
        toggleRemoveContainer(show) {
            this.showRemoveCont = show
        },
        removeCont() {
            this.isLoading = true
            $fetch("/api/Docker", {
                method: "DELETE",
                body: {
                    name: this.name
                },
                headers: {
                    'Cookie': 'username=' + this.cookies["username"]
                }
            }).then(res => {
                if (res.status != undefined) {
                    navigateTo("/")
                } else {
                    this.isLoading = false
                }
                //navigateTo("/status?name=" + this.name)
            }).catch(e => {
                this.isLoading = false
                console.log(e)
            })
        }
    }
}
</script>

<style>
.removeBackground {
    background-color: rgb(0, 0, 0, 0.5);
    height: 100%;
    width: 100%;
    position: fixed;
    top: 0;
    left: 0;
    z-index: 3;
}

.removeContWindow {
    width: 400pt;
    height: 300pt;
    position: fixed;
    top: calc(50% - 150pt);
    left: calc(50% - 200pt);
    background-color: rgb(35, 35, 45);
    align-content: center;
    text-align: center;
    border-radius: 25pt;
}

.removeCont {
    position: absolute;
    top: 5pt;
    right: 75pt;
    width: 32pt;
    height: 32pt;
    border-radius: 20pt;
    border: solid 3pt rgb(155, 255, 170);
    font-size: 27pt;
    text-align: center;
    align-items: center;
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    user-select: none;
}

.removeCont:hover {
    animation: hoverProfil 0.05s linear forwards;
    color: rgb(35, 35, 45);
    font-weight: 1000;
}

.confirmRemBtn {
    width: 144pt;
    height: 34pt;
    border-radius: 20pt;
    border: solid 3pt rgb(155, 255, 170);
    background-color: transparent;
    font-family: inherit;
    font-size: inherit;
    color: inherit;
    font-weight: inherit;
    margin-left: 25pt;
    margin-right: 25pt;
    margin-top: 25pt;
}

.confirmRemBtn:hover {
    background-color: rgb(155, 255, 170);
    color: black;
}

.cancelRemBtn {
    width: 144pt;
    height: 34pt;
    border-radius: 20pt;
    border: solid 3pt rgb(255, 70, 70);
    background-color: transparent;
    font-family: inherit;
    font-size: inherit;
    color: inherit;
    font-weight: inherit;
    margin-left: 25pt;
    margin-right: 25pt;
    margin-top: 25pt;
}

.cancelRemBtn:hover {
    background-color: rgb(255, 70, 70);
}

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
    background-color: rgb(255, 70, 70);
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