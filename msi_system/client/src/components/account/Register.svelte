<script lang="ts">
    import type IUserResiterDto from '../../interfaces/dtos/IUserRegisterDto';
    import * as dbService from '../../services/pocketbase.service';

    let dto = {
        username: '',
        email: '',
        password: '',
        passwordConfirm: ''
    } as IUserResiterDto;

    function handleSubmit() {
        dbService.createUser(dto);
    }
</script>

<section class="flex">
    <fieldset>
        <legend>Login</legend>
        <form on:submit|preventDefault={handleSubmit}>
            <label>
                <input bind:value={dto.username} placeholder="username" type="text">
            </label>
            <label>
                <input bind:value={dto.email} placeholder="email" type="text">
            </label>
            <label>
                <input bind:value={dto.password} placeholder="password" type="password">
            </label>
            <label>
                <input bind:value={dto.passwordConfirm} placeholder="confirm password" type="password">
            </label>
            <div class="flex">
                <button type="submit">Submit</button>
            </div>
        </form>
    </fieldset>
</section>