<script lang="ts">
    import type { IHomeComponent } from "../../interfaces/models/IHomeComponent";
    import { currentComponentChanged } from "../../stores/home/homenav.store";
    import HomeNav from "./HomeNav.svelte";
    
    let current: IHomeComponent;
    currentComponentChanged(x => current = x);
</script>

<section>
    <HomeNav />
    <div>
        <svelte:component this={current.component} />
    </div>
</section>

<style>
    section {
        display: flex;
        height: 100%;
    }
    div {
        width: 100%;
    }
</style>