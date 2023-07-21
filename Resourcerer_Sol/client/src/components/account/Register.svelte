<script lang="ts">
    import * as userController from '../../controllers/user.controller';
    import * as pageService from '../../stores/commonUi/page.service';
    import type { IAppUserDto } from '../../interfaces/dtos/interfaces';

    let model = {
        dto: {
            name: '',
            password: '',
        } as IAppUserDto,
        passwordConfirm: ''
    };

    function handleSubmit() {
        const validate = (x: string) => x && x.length >= 2;
        let errors = [];

        if(!validate(model.dto.name)) errors.push('Name must be at least 2 chars long'); 
        if(!validate(model.dto.password)) errors.push('Password must be at least 2 chars long');
        if(!validate(model.passwordConfirm)) errors.push('Password confirm must be at least 2 chars long');
        if(model.dto.password !== model.passwordConfirm) errors.push(`Password confirmation doesn't match`);
        
        if(errors.length > 0) {
            console.log(errors);
            return;
        }

        userController.register(model.dto, () => {
            pageService.goto.settings();
        });
    }

    function goToLoginPage() {
        pageService.goto.login();
    }
</script>

<section class="flex">
    <fieldset>
        <legend>Register</legend>
        <form on:submit|preventDefault={handleSubmit}>
            <label>
                <input bind:value={model.dto.name} placeholder="name" type="text">
            </label>
            <label>
                <input bind:value={model.dto.password} placeholder="password" type="password">
            </label>
            <label>
                <input bind:value={model.passwordConfirm} placeholder="confirm password" type="password">
            </label>
            <div class="flex">
                <button type="submit">Submit</button>
            </div>
            <div class="login">
                <i>Or <a href="#a" on:click={goToLoginPage}>login</a> if you already have an account</i>
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

    .login {
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