<script lang="ts">
    import { onMount } from "svelte";
    import { onNotificationsUpdated, clearNotifications, type INotification } from "../../stores/commonUi/notification.store";

    let notifications: { head: INotification, tail: INotification[] } = {
        head: {} as INotification,
        tail: []
    };

    let visible = false;
    
    onMount(() => {
        onNotificationsUpdated(ns => {
            const [x, ...xs] = ns;
            console.log(ns);
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
    <div class="messages-flex">
        <div class="scroller">
            <div class="message-list">
                <div class="message-item-flex">
                    <div>{notifications.head.text}</div>
                    <div class="severity-status"></div>
                </div>
                {#each notifications.tail as msg}
                    <div class="message-item-flex">
                        <div>{msg.text}</div>
                        <div class="severity-status"></div>
                    </div>
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

    .messages-flex {
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .message-item-flex {
        display: flex;
        justify-content: space-around;
        align-items: center;
    }

    .scroller {
        width: 50%;
        position: relative;
        line-height: 1rem;
        min-height: 1rem;
        max-height: 1rem;
        padding-left: .5rem;
        background-color: var(--color-white);
        color: var(--color-black);
        overflow-y: hidden;
        transition: .3s;
    }

    .scroller:hover {
        overflow-y: visible;
    }

    .scroller:hover > .message-list {
        overflow-y: scroll;
        max-height: 10rem;
    }

    .message-list {
        width: 100%;
        left: 0;
        position: absolute;
        background-color: inherit;
        z-index: 100;
        max-height: 1rem;
        transition: .3s;
        scrollbar-gutter: stable both-edges;
    }

    .message-list > div {
        margin-bottom: .5rem;
        padding-left: .5rem;
    }

    .severity-status {
        content: ' ';
        min-height: .7rem;
        min-width: .7rem;
        background-color: red;
    }

    button {
        margin-left: 1rem;
    }
</style>