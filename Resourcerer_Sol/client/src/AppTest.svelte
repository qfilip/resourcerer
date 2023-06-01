<script lang="ts">
    import * as pageLoaderService from './services/commonUi/loader.service';
    
    function testLoader() {
        [2500, 1000, 1500].forEach(x => {
            pageLoaderService.show();
            const tout = setTimeout(() => {
                pageLoaderService.hide();
                console.log(`task ${x} done`);
                clearTimeout(tout);
            }, x)
        })
    }
</script>

<button on:click={testLoader}>Test loader</button>

<style>
</style>