import * as loaderService from '../services/commonUi/loader.service';

export const apiUrl = 'http://localhost:5128';

export async function handlePromise<T>(apiCall: () => Promise<T>, message?: string) {
    loaderService.show(message);
    try {
        const data = await apiCall();
        loaderService.hide();
        return data;
    }
    catch(error) {
        loaderService.hide();
        console.log(error);
    }
}

export async function executeGet<TResponse>(url: string, responseHandler: (Response) => TResponse = null) {
    const response = await fetch(url);
    console.log(response);
    if(responseHandler) {
        return responseHandler(response);
    }

    return null;
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