export default interface IUserRegisterDto {
    id: string;
    username: string;
    email: string;
    emailVisibility: boolean;
    password: string;
    passwordConfirm: string;
    verified: boolean;
}