<script lang="ts">
    import type { ICategoryDto } from "../../interfaces/dtos/interfaces";


    export let category: ICategoryDto = null;
    let focusedBtn = 0;

    function setFocus(btnIdx: number) {
        focusedBtn = btnIdx;
    }
</script>

{#if category}
    <div>
        <div class="btn-flex">
            <button on:click={() => setFocus(0)} class="{focusedBtn === 0 ? 'focused' : ''}">
                Elements
            </button>
            <button on:click={() => setFocus(1)} class="{focusedBtn === 1 ? 'focused' : ''}">
                Composites
            </button>
        </div>
        <div>
            
        </div>
    </div>
{/if}

<style>
    .btn-flex {
        display: flex;
        justify-content: space-between;
    }

    button {
        border: none;
        width: 50%;
        color: var(--color-white);
        background-color: var(--color-black);
    }

    .focused, button:hover {
        background-color: var(--color-red);
    }
</style>