<script lang="ts">
    import { onMount } from "svelte";
    import type IAppUser from '../../interfaces/dbModels/IAppUser';
    import * as pageService from '../../services/page.service';
    
    onMount(() => {
        pageService.goto.home();
    });

    let dto = {
        email: '',
        password: ''
    } as IAppUser;

    function handleSubmit() {
        console.log(dto);
        pageService.goto.home();
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
                <input bind:value={dto.email} placeholder="email" type="text">
            </label>
            <label>
                <input bind:value={dto.password} placeholder="password" type="password">
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
        font-size: .5rem;
    }

    .flex {
        display: flex;
        justify-content: center;
        align-items: center;
    }
</style>