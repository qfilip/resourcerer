<script lang="ts">
    import { onMount } from "svelte";
    import type IUserLoginDto from "../../interfaces/dtos/IUserLoginDto";
    import * as pageService from '../../services/commonUi/page.service';
    import * as userController from '../../controllers/user.controller';
    
    onMount(() => {
        userController.checkAuthStore(() => {
            pageService.goto.home();
        })
    });

    let dto = {
        email: 'admin@admin.com',
        password: 'adminadmin',
        asAdmin: true
    } as IUserLoginDto;

    function handleSubmit() {
        console.log(dto);
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
                <input bind:value={dto.email} placeholder="email" type="text" />
            </label>
            <label>
                <input bind:value={dto.password} placeholder="password" type="password" />
            </label>
            <label>
                As admin <input bind:checked={dto.asAdmin} type="checkbox" />
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