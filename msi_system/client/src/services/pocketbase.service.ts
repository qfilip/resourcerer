import PocketBase from 'pocketbase';
import type IUserRegisterDto from '../interfaces/dtos/IUserRegisterDto';

const pb = new PocketBase('http://127.0.0.1:8090');

export function createUser(dto: IUserRegisterDto) {
    const promise = pb.collection('users').create(dto);

    promise.then(x => console.log(x)).catch(err => console.log(err));
}