<script lang="ts">
    import { onMount } from "svelte";
    import * as pageService from '../../stores/commonUi/page.store';
    import * as userController from '../../controllers/user.controller';
    import { onUserChanged } from "../../stores/user.store";
    import type { IAppUserDto } from "../../interfaces/dtos/interfaces";
    
    onMount(() => {
        onUserChanged(x => {
            if(!x) return;
            pageService.goto.home();
        });
    });

    let dto = {
        name: 'admin',
        password: 'admin',
        permissions: {}
    } as IAppUserDto;

    function handleSubmit() {
        userController.login(dto, () => pageService.goto.home());
    }

    function goToRegisterPage() {
        pageService.goto.register();
    }
</script>

<section class="flex">
    <fieldset>
        <legend>Login</legend>
        <form on:submit|preventDefault={handleSubmit}>
            <label>
                <input bind:value={dto.name} placeholder="email" type="text" />
            </label>
            <label>
                <input bind:value={dto.password} placeholder="password" type="password" />
            </label>
            <button type="submit">Submit</button>
            <div class="register">
                <i>Or <a href="#a" on:click={goToRegisterPage}>register</a> if you don't have an account</i>
            </div>
        </form>
    </fieldset>
</section>

<style>
    section {
        height: 80vh;
    }

    legend {
        padding: 0 var(--padding);
        color: var(--color-white);
        background-color: var(--color-black);
    }

    button {
        width: 100%;
    }

    .register {
        margin-top: .2rem;
        width: 100%;
        text-align: center;
        font-size: .7rem;
    }

    .flex {
        display: flex;
        justify-content: center;
        align-items: center;
    }
</style>