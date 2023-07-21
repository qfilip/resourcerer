<script lang="ts">
    import * as pageLoaderService from './stores/commonUi/loader.service';
    import * as notificationStore from './stores/commonUi/notification.store';
    
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

    let notificationCounter = 0;
    function testNotification() {
        notificationCounter++;
        notificationStore.addNotification(`Notification number ${notificationCounter}`)
    }
</script>

<button on:click={testLoader}>Test loader</button>
<button on:click={testNotification}>Test Notification</button>

<style>
</style>