<template>
  <NavBar showProfile=true>Your Profile</NavBar>
  <div class="StatusContainer">
    <div class="userName">
      {{ Username }}
    </div>
    <div class="buttonContainer">
      <button class="changePasswordBtn" @click="togglePassword(true)">
        Change Password
      </button>
    </div>
    <div class="buttonContainer">
      <button class="deleteActionBtn" @click="toggleDelete(true)">
        Delete Account
      </button>
    </div>
    <div class="buttonContainer">
      <button class="deleteActionBtn" @click="Logout()">
        Logout
      </button>
    </div>
  </div>
  <div class="deleteBackground" v-if="showDelete || showPassword">
    <div class="deleteWindow" v-if="showDelete">
      Do you really want to delete the account?
      <button class="deleteCancelBtn" @click="toggleDelete(false)">
        Cancel
      </button>
      <button class="deleteConfirmBtn" @click="confirmDelete()">
        Confirm
      </button>
    </div>
    <div class="changePwWindow" v-if="showPassword">
      Change Password
      <div class="buttonContainer"><input type="password" placeholder="Enter Password..." v-model="Pw" /></div>
      <div class="buttonContainer"><input type="password" placeholder="Repeat Password..." v-model="repeatedPw" /></div>
      <div v-if="showPwErr" class="PwErr">
        Password and repeated password don't match!
      </div>
      <button class="changePwCancelBtn" @click="togglePassword(false)">
        Cancel
      </button>
      <button class="changePwConfirmBtn" @click="confirmPw()">
        Confirm
      </button>
    </div>
  </div>
</template>

<script>
import checkAuth from '~/utils/auth';
import getCookies from '~/utils/getCookies';

export default {
  data() {
    return {
      showDelete: false,
      showPassword: false,
      Pw: "",
      repeatedPw: "",
      Username: "",
      showPwErr: false
    }
  },
  mounted() {
    if (!checkAuth()) navigateTo("/Login")

    this.Username = getCookies()["username"]
  },
  methods: {
    toggleDelete(show) {
      this.showDelete = show
    },
    togglePassword(show) {
      this.showPassword = show
      this.Pw = ""
      this.repeatedPw = ""
      this.showPwErr = false
    },
    Logout(){
        document.cookie = "username=" + this.Username + "; Max-Age=0"
        navigateTo("/Login")
    },
    confirmPw() {
      $fetch("/api/User", {
        method: "PUT",
        body: {
          password: this.Pw
        },
        headers: {
          'Cookie': 'username=' + this.Username
        }
      }).then(res => {
        console.log('Confirmed new password')
        if (this.Pw == this.repeatedPw) {
          this.togglePassword(false);
          console.log('Confirmed');
        } else {
          this.showPwErr = true;
        }
      })

    },
    confirmDelete() {
      console.log('Confirmed deletion')
      $fetch("/api/User", {
        method: "DELETE",
        body: {
          name: this.Username
        }
      }).then(res => {
        if (res.deleted != undefined && res.deleted == true) {
          document.cookie = ""
          navigateTo("/Login")
        } else {
          this.toggleDelete(false)
          console.log("Something went Wrong :(")
        }
      }).catch(e => {
        this.toggleDelete(false)
        console.log(e)
      })
    }
  }
}
</script>

<style scoped>
.userName {
  text-align: center;
  font-size: 25pt;
  font-weight: 600;
  overflow: auto;
  white-space: nowrap;
}

.StatusContainer {
  position: relative;
  margin: auto;
  background: radial-gradient(closest-side, rgb(65, 65, 75), rgb(35, 35, 45));
  min-width: 200pt;
  width: calc(50vw - 50pt);
  max-width: 600pt;
  height: calc(100vh - 130pt);
  margin-top: 15pt;
  border-radius: 15pt;
  padding: 25pt;
  justify-content: center;
}

.userName::-webkit-scrollbar {
  background: transparent;
}

.userName::-webkit-scrollbar-thumb {
  background: rgb(155, 255, 170);
  border-radius: 5pt;
}

.buttonContainer {
  width: 100%;
}

.deleteActionBtn {
  width: 194pt;
  height: 34pt;
  border-radius: 20pt;
  border: solid 3pt rgb(155, 255, 170);
  background-color: transparent;
  font-family: inherit;
  font-size: inherit;
  color: inherit;
  font-weight: inherit;
  margin-top: 25pt;
  margin-left: calc(50% - 100pt);
}

.deleteActionBtn:hover {
  background-color: rgb(155, 255, 170);
  color: #000;
}

.deleteConfirmBtn {
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

.deleteConfirmBtn:hover {
  background-color: rgb(155, 255, 170);
  color: black;
}

.deleteCancelBtn {
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

.deleteCancelBtn:hover {
  background-color: rgb(255, 70, 70);
}

.deleteBackground {
  background-color: rgb(0, 0, 0, 0.5);
  height: 100%;
  width: 100%;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 3;
}

.deleteWindow {
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

.changePasswordBtn {
  width: 194pt;
  height: 34pt;
  border-radius: 20pt;
  border: solid 3pt rgb(155, 255, 170);
  background-color: transparent;
  font-family: inherit;
  font-size: inherit;
  color: inherit;
  font-weight: inherit;
  margin-top: 25pt;
  margin-left: calc(50% - 100pt);
}

.changePasswordBtn:hover {
  background-color: rgb(155, 255, 170);
  color: #000;
}

.changePwConfirmBtn {
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

.changePwConfirmBtn:hover {
  background-color: rgb(155, 255, 170);
  color: black;
}

.changePwCancelBtn {
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

.changePwCancelBtn:hover {
  background-color: rgb(255, 70, 70);
}

.changePwWindow {
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
  margin-top: 25pt;
  padding-left: 10pt;
  padding-right: 10pt;
}

.PwErr {
  color: rgb(255, 70, 70);
  margin-top: 25pt;
}
</style>