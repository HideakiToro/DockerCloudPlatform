<template>
  <NavBar>Your Containers</NavBar>
  <div class="CardGrid">
    <div class="Card" v-for="(item, index) in 50" @click="navigateDetails()">
      <div class="CardTitle">This is Container #{{ index }}</div>
      <div v-if="Math.random() > 0.8" class="CardStatus"></div>
      <div v-else class="CardStatus OkStatus"></div>
    </div>
  </div>
</template>

<style>
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