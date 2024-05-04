<template>
  <div class="NavBar">
    Your Containers
    <div class="Profil"></div>
  </div>
  <div class="CardGrid">
    <div class="Card" v-for="(item, index) in 50" @click="navigateDetails()">
      <div class="CardTitle">This is Container #{{ index }}</div>
      <div v-if="Math.random() > 0.8" class="CardStatus"></div>
      <div v-else class="CardStatus OkStatus"></div>
    </div>
  </div>
</template>

<style>
body {
  background-color: rgb(55, 55, 70);
  color: rgb(155, 255, 170);
  font-family: Roboto;
  font-size: 16pt;
  font-weight: 500;
  z-index: 1;
  padding: 0;
  margin: 0;
  height: 100%;
}

.NavBar {
  width: calc(100%-30pt);
  height: 50pt;
  align-content: center;
  padding-left: 15pt;
  padding-right: 15pt;
  background-color: rgb(35, 35, 45);
  z-index: 2;
  border-bottom: solid 3pt rgb(155, 255, 170);
  position: sticky;
  top: 0;
}

.Profil {
  position: absolute;
  top:5pt;
  right: 25pt;
  width: 32pt;
  height: 32pt;
  border-radius: 20pt;
  border: solid 3pt rgb(155, 255, 170);
}

.Profil:hover {
  animation: hoverProfil 0.05s linear forwards;
}

@keyframes hoverProfil {
  from {
    background-color: transparent;
  }
  to {
    background-color: rgb(155, 255, 170);
  }
}

.CardGrid {
  width: 100%;
  height: 100%;
  margin-top: 12.5pt;
  display: grid;
  grid-template-columns: repeat(auto-fit, 250pt);
  place-content: center;
}

.Card {
  width: 150pt;
  height: 100pt;
  padding: 25pt;
  padding-bottom: 22pt;
  margin-top: 12.5pt;
  margin-bottom: 12.5pt;
  background-color: rgb(35, 35, 45);
  border-radius: 10pt;
  text-align: center;
  border-bottom: solid 3pt rgb(155, 255, 170);
  overflow: hidden;
}

.Card:hover {
  animation: hoverCard 0.05s ease-in forwards;
}

@keyframes hoverCard {
  from {
    border-bottom: solid 3pt rgb(155, 255, 170);
    padding-bottom: 22pt;
  }
  to {
    border-bottom: solid 15pt rgb(155, 255, 170);
    padding-bottom: 10pt;
  }
}

.CardTitle {
  height: 40pt;
  overflow: hidden;
  text-overflow: ellipsis;
}

.CardStatus {
  color: white;
  margin-top: 20pt;
  padding-top: 10pt;
  background-color: rgb(255, 70, 70);
  height: 30pt;
  border-radius: 15pt;
  position: inherit;
  bottom: 0;
}

.OkStatus {
  background-color: rgb(70, 255, 110);
}
</style>

<script>
export default {
  methods: {
    navigateDetails() {
      window.open("/Status", "_self")
    }
  }
}
</script>