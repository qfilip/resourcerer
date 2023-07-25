<script lang="ts">
    import { onMount } from "svelte";
    import * as modalStore from '../../stores/commonUi/modal.store';
    import type { IModalOptions } from "../../interfaces/models/IModalOptions";

    let dialog: HTMLDialogElement;
    let options: IModalOptions = modalStore.defaultOptions;
    
    onMount(() => {
        modalStore.onOpen(x => {
            if(x.open) {
                options = x;
                dialog.showModal();
            }
            else {
                dialog.close();
            }
        });
    })
</script>

<dialog bind:this={dialog}>
    <h4>{options.header}</h4>
    <slot name="content"></slot>
    <div class="dialog-footer">
        {#each options.buttons as btn}
            <button on:click={() => btn.onClick()}>{ btn.text }</button>
        {/each}
    </div>
</dialog>

<style>
    dialog {
        padding: var(--padding);
        width: 30%;
    }

    .dialog-footer {
        display: flex;
        justify-content: flex-end;
        align-items: center;
    }
</style>