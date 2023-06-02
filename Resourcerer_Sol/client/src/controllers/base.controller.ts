import type IApiDto from '../interfaces/dtos/IApiDto';
import * as loaderService from '../services/commonUi/loader.service';

const apiUrl = 'https://localhost:44387';

export function sendGet(endpoint: string, message?: string) {
    const url = apiUrl + endpoint;
    const apiCall = () => fetch(url);

    return handleCall(apiCall, message);
}

export function sendPost(endpoint: string, body: unknown, message?: string) {
    const url = apiUrl + endpoint;
    
    const options = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    } as RequestInit;
    
    const apiCall = () => fetch(url, options);

    return handleCall(apiCall, message);
}

async function handleCall<TResponse>(apiCall: () => Promise<Response>, message?: string) {
        loaderService.show(message);
        try {
            const response = await apiCall();
            loaderService.hide();
            
            if(response.status >= 200 || response.status < 300) {
                return await response.json();
            }
            else {
                console.warn(response.status);
            }
        }
        catch(error) {
            loaderService.hide();
            console.log(error);
            return {
                status: 500
            } as IApiDto<TResponse>;
        }
}