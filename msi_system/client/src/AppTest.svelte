<script lang="ts">
    import Modal from "./components/commonUI/Modal.svelte";
    import * as modalService from './services/modal.service';
    import * as utils from './utils';

    function openDialog() {
        modalService.open();
    }

    async function seedDb() {
        await utils.seedDbAsync();
    }
</script>
<Modal>
    <div slot="content">
        Hello
    </div>
</Modal>
<button on:click={openDialog}>Open dialog</button>
<button on:click={seedDb}>Seed Db</button>


<style>
</style>