<template>
  <NavBar>
    Your Containers
    <div class="AddContainer" @click="toggleAddContainer(true)">
      +
    </div>
  </NavBar>
  <div class="CardGrid">
    <div class="Card" v-for="(container, index) in containers" @click="navigateDetails()">
      <div class="CardTitle">{{ container }}</div>
      <div v-if="Math.random() > 0.8" class="CardStatus"></div>
      <div v-else class="CardStatus OkStatus"></div>
    </div>
  </div>
  <div class="addBackground" v-if="showCount">
    <div class="addContWindow">
      hello World
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
  background: radial-gradient(closest-side, rgb(65, 65, 75), rgb(35, 35, 45));
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

.AddContainer {
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

.AddContainer:hover {
  animation: hoverProfil 0.05s linear forwards;
  color: rgb(35, 35, 45);
  font-weight: 1000;
}

.addBackground {
  background-color: rgb(0, 0, 0, 0.5);
  height: 100%;
  width: 100%;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 3;
}

.addContWindow {
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
</style>

<script>
export default {
  data() {
    return {
      showCount: false,
      containers: []
    }
  },
  mounted() {
    $fetch("/api/Docker").then(res => {
      this.containers = res;
    }).catch(e => {
      console.log(e);
      this.containers = [];
    })
  },
  methods: {
    navigateDetails() {
      window.open("/Status", "_self")
    },
    toggleAddContainer(show) {
      this.showCount = show
    }
  }
}
</script>