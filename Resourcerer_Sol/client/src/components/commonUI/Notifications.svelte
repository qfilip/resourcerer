<script lang="ts">
    import { onMount } from "svelte";
    import type { INotification } from "../../interfaces/models/INotification";
    import { eSeverity } from "../../interfaces/enums/eSeverity";
    import { clearNotifications, notificationsChangedEvent } from "../../stores/commonUi/notification.store";

    let notifications: { head: INotification, tail: INotification[] } = {
        head: null,
        tail: []
    };

    let visible = false;
    let latestSeverity = eSeverity.Info;

    onMount(() => {
        notificationsChangedEvent(ns => {
            const [x, ...xs] = ns;
            
            notifications.head = x;
            notifications.tail = xs;
            
            if(ns.length > 0) {
                latestSeverity = notifications.head.severity;
                visible = true;
            }
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

<section class="{visible ? 'visible' : 'hidden'}"
    class:b-blue={latestSeverity === eSeverity.Info}
    class:b-orange={latestSeverity === eSeverity.Warning}
    class:b-error={latestSeverity === eSeverity.Error}>
    <div class="messages-flex">
        <div class="scroller">
            <div class="message-list">
                {#if notifications.head}
                <div class="message-item-flex">
                    <div class="message-text">
                        {notifications.head.text}
                    </div>
                    <div class="severity-status"
                    class:b-blue={notifications.head.severity === eSeverity.Info}
                    class:b-orange={notifications.head.severity === eSeverity.Warning}
                    class:b-error={notifications.head.severity === eSeverity.Error}></div>
                </div>
                {/if}
                {#each notifications.tail as msg}
                    <div class="message-item-flex">
                        <div class="message-text">{msg.text}</div>
                        <div class="severity-status"
                        class:b-blue={notifications.head.severity === eSeverity.Info}
                        class:b-orange={notifications.head.severity === eSeverity.Warning}
                        class:b-error={notifications.head.severity === eSeverity.Error}></div>
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
        margin-bottom: .5rem;
        padding-left: .5rem;
    }

    .message-text {
        width: 90%;
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
    }

    .severity-status {
        content: ' ';
        min-height: .7rem;
        min-width: .7rem;
        border: .1rem groove var(--color-black);
    }

    button {
        margin-left: 1rem;
        border: none;
    }
</style>