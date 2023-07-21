<script lang="ts">
    import { onMount } from "svelte";
    import { onNotificationsUpdated, clearNotifications } from "../../stores/commonUi/notification.store";

    let notifications: { head: string, tail: string[] } = { head: '', tail: [] };
    let visible = false;
    
    onMount(() => {
        onNotificationsUpdated(ns => {
            const [x, ...xs] = ns;
            
            notifications.head = x;
            notifications.tail = xs;
            
            if(ns.length > 0) visible = true;
        });
    });

    function hide() {
        visible = false;
    }

    function clear() {
        visible = false;
        clearNotifications();
    }
</script>

<section class="{visible ? 'visible' : 'hidden'}">
    <!-- <h2>Notifications</h2> -->
    <div class="message-flex">
        <div class="scroller">
            {notifications.head}
            <div class="old-messages">
                {#each notifications.tail as msg}
                    <div>{msg}</div>
                {/each}
            </div>
        </div>
        <button on:click={clear}>Clear</button>
        <button on:click={hide}>Hide</button>
    </div>
</section>


<style>
    section {
        padding: .2rem .2rem;
        font-size: .8rem;
        display: flex;
        justify-content: space-between;
        transition: .3s;
        background-color: var(--color-red);
    }

    .hidden {
        transform: rotateX(90deg);
    }

    .visible {
        transform: rotateX(0deg);
    }

    .message-flex {
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .scroller {
        width: 50%;
        position: relative;
        line-height: 1rem;
        min-height: 1rem;
        max-height: 1rem;
        padding-left: .5rem;
        /* background-color: var(--color-white); */
        background-color: var(--color-red);
        color: var(--color-white);
        overflow-y: hidden;
        transition: .3s;
    }

    .scroller:hover {
        overflow-y: visible;
    }

    .scroller:hover > .old-messages {
        overflow-y: scroll;
        max-height: 10rem;
    }

    .old-messages {
        width: 100%;
        /* top: 1rem; */
        left: 0;
        position: absolute;
        background-color: inherit;
        z-index: 100;
        max-height: 1rem;
        transition: .3s;
        /* overflow-y: auto; */
        transition: .3s;
    }

    .old-messages > div {
        padding-left: .5rem;
    }

    button {
        margin-left: 1rem;
    }
</style>