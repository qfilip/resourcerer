import IApiDto from '../interfaces/dtos/IApiDto';
import * as loaderService from '../services/commonUi/loader.service';

const apiUrl = 'https://localhost:44387';

export function sendGet<TResponse>(endpoint: string, onOk: (r: Response) => TResponse, message?: string) {
    const url = apiUrl + endpoint;
    const apiCall = () => fetch(url);

    return handleCall(apiCall, onOk, message);
}

export function sendPost<TResponse>(endpoint: string, body: unknown, onOk: (r: Response) => TResponse, message?: string) {
    const url = apiUrl + endpoint;
    
    const options = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    } as RequestInit;
    
    const apiCall = () => fetch(url, options);

    return handleCall(apiCall, onOk, message);
}

export async function executePost<TRequest, TResponse>(url: string, data: TRequest, responseHandler: (Response) => TResponse = null) {
    const response = await fetch(url, {
        method: "POST",
        mode: "cors",
        cache: "no-cache",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data)
      });

      if(responseHandler) {
        return responseHandler(response);
      }

      return await response.json() as TResponse;
}

async function handleCall<TResponse>(
    apiCall: () => Promise<Response>,
    onOk: (r: Response) => TResponse,
    message?: string) {
        loaderService.show(message);
        try {
            const response = await apiCall();
            loaderService.hide();
            
            const data = (response.status === 200) ? onOk(response) : null;
            const dto: IApiDto<TResponse> = {
                status: response.status,
                data: data
            };

            return dto;
        }
        catch(error) {
            loaderService.hide();
            console.log(error);
            return {
                status: 500
            } as IApiDto<TResponse>;
        }
}