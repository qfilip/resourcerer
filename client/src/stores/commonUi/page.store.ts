import Login from '../../components/account/Login.svelte';
import Register from '../../components/account/Register.svelte';
import Home from '../../components/home/Home.svelte';
import { writable } from "svelte/store";
import type { IAppPage } from '../../interfaces/models/IAppPage';
import type { PageName } from '../../interfaces/types/PageName';

export const pages: IAppPage[] = [
    { 
        name: '',
        component: Login,
    },
    { 
        name: 'Login',
        component: Login,
    },
    { 
        name: 'Register',
        component: Register,
    },
    { 
        name: 'Home',
        component: Home,
    }
];

const currentPage$ = writable<IAppPage>(pages[0]);

export function changePage(name: PageName) {
    const page = pages.find(x => x.name === name);
    currentPage$.set(page);
}

export const onCurrentPageChanged = currentPage$.subscribe;

export const goto = {
    login: function() {
        changePage('Login');
    },
    register: function() {
        changePage('Register');
    },
    home: function() {
        changePage('Home');
    }
}