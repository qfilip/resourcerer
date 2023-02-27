<script lang="ts">
    import { onMount } from "svelte";
    import type { IModalButton, IModalOptions } from "../../services/commonUi/modal.service";
    import * as modalService from '../../services/commonUi/modal.service';

    let dialog: HTMLDialogElement;
    let options: IModalOptions = modalService.defaultOptions;
    
    onMount(() => {
        modalService.onOpen(x => {
            if(!x.open) return;

            options.buttons = x.buttons.length > 0 ?
                x.buttons :
                [{ text: 'Close', onClick: closeDialog }]
            
                dialog.showModal();
        });
    })

    function closeDialog() {
        dialog.close();
    }
</script>

<dialog bind:this={dialog}>
    <slot name="content"></slot>
    <div class="dialog-footer">
        {#each options.buttons as btn}
            <button on:click={btn.onClick}>{ btn.text }</button>
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