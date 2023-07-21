<script lang="ts">
    import { onMount } from 'svelte';
    import type IPageloaderOptions from '../../stores/commonUi/loader.service'
    import * as pageLoaderService from '../../stores/commonUi/loader.service';

    let options = {} as IPageloaderOptions;
    onMount(() => {
        pageLoaderService.options(x => options = x);
    });
</script>

{#if options?.open}
    <div class="background">
        <div class="container">
            <div class="lds-ripple">
                <div></div>
                <div></div>
            </div>
        </div>

        {#if options?.message}
            <div class="info-box">{options.message}</div>
        {/if}
        {#if options?.progress}
            <div class="info-box">{options.progress}</div>
        {/if}
    </div>
{/if}

<style>
    .background {
        position: absolute;
        z-index: 1000;
        width: 100%;
        height: 100%;
        background-color: #00000099;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        pointer-events: none;
    }

    .container {
        position: relative;
        min-width: 50%;
        min-height: 30%;

        display: flex;
        align-items: center;
        justify-content: center;
        color: var(--color-white);
    }

    .info-box {
        font-size: 1.5rem;
        color: var(--color-white);
        animation: text-glow 2s infinite;
    }

    @keyframes text-glow {
        0% {
            text-shadow: .1rem .1rem .2rem var(--color-white);
        }
        50% {
            text-shadow: .1rem .1rem .5rem var(--color-black);
        }
        100% {
            text-shadow: .1rem .1rem .2rem var(--color-white);
        }
    }

    .lds-ripple,
    .lds-ripple div {
        box-sizing: border-box;
    }
    .lds-ripple {
        display: inline-block;
        position: relative;
        width: 80px;
        height: 80px;
    }
    .lds-ripple div {
        position: absolute;
        border: 4px solid currentColor;
        opacity: 1;
        border-radius: 50%;
        animation: lds-ripple 1s cubic-bezier(0, 0.2, 0.8, 1) infinite;
    }
    .lds-ripple div:nth-child(2) {
        animation-delay: -0.5s;
    }
    @keyframes lds-ripple {
        0% {
            top: 36px;
            left: 36px;
            width: 8px;
            height: 8px;
            opacity: 0;
        }
        4.9% {
            top: 36px;
            left: 36px;
            width: 8px;
            height: 8px;
            opacity: 0;
        }
        5% {
            top: 36px;
            left: 36px;
            width: 8px;
            height: 8px;
            opacity: 1;
        }
        100% {
            top: 0;
            left: 0;
            width: 80px;
            height: 80px;
            opacity: 0;
        }
    }
</style>
