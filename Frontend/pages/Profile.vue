<template>
  <NavBar>Your Profile</NavBar>
  <div>
    <div>
      User
    </div>
    <button class="deleteActionBtn" @click="toggleDelete(true)">
      Delete Account
    </button>
  </div>
  <div class="deleteBackground" v-if="showDelete">
    <div class="deleteWindow">
        Do you really want to delete the account?
        <button class="deleteCancelBtn" @click="toggleDelete(false)">
          Cancel
        </button>
        <button class="deleteConfirmBtn" @click="console.log('Confirmed Deletion'); toggleDelete(false)">
          Confirm
        </button>
    </div>
  </div>
</template>

<script>
  export default{
    data(){
      return{
        showDelete:false
      }
    },
    methods:{
      toggleDelete(show){
        this.showDelete = show
      }
    }
  }
</script>

<style scoped>
  .deleteActionBtn{
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
  }
  .deleteActionBtn:hover{
    background-color: rgb(155, 255, 170);
    color: #000;
  }

  .deleteConfirmBtn{
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
  .deleteConfirmBtn:hover{
    background-color: rgb(155, 255, 170);
    color: black;
  }

  .deleteCancelBtn{
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
  .deleteCancelBtn:hover{
    background-color:rgb(255, 70, 70);
  }
  
  .deleteBackground{
    background-color: rgb(0,0,0,0.5);
    height: 100%;
    width: 100%;
    position: fixed;
    top: 0;
    left: 0;
    z-index: 3;
  }
  .deleteWindow{
    width: 400pt;
    height: 300pt; 
    position: fixed;
    top:calc(50% - 150pt);
    left:calc(50% - 200pt);
    background-color: rgb(35, 35, 45);
    align-content: center;
    text-align: center;
    border-radius: 25pt;
  }
</style>