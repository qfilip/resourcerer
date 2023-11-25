import axios, { AxiosError } from 'axios';
import * as loaderService from '../stores/commonUi/loader.service';
import { addNotification } from '../stores/commonUi/notification.store';
import { eSeverity } from '../interfaces/enums/eSeverity';
import type { INotification } from '../interfaces/models/INotification';

const apiUrl = 'https://localhost:44387/api/1.0';

const axi = axios.create();
const axiSilent = axios.create(); // no page loader

export function setInterceptor(jwt) {
  axi.interceptors.request.clear();
  axi.interceptors.request.use(
      (config) => {
        loaderService.show();
        config.url = apiUrl + config.url;
        config.headers.Authorization = `Bearer ${jwt}`;
        return config;
      },
      (error) => {
        loaderService.hide();
        console.warn(error);
        return Promise.reject(error);
      }
    );

    axiSilent.interceptors.request.clear();
    axiSilent.interceptors.request.use(
      (config) => {
        config.url = apiUrl + config.url;
        config.headers.Authorization = `Bearer ${jwt}`;
        return config;
      },
      (error) => {
        console.warn(error);
        return Promise.reject(error);
      }
    );
}

axi.interceptors.response.use((response) => {
  loaderService.hide();
  return response;
}, (error) => {
  loaderService.hide();
  handleErrorResponse(error);
  return Promise.reject(error);
});

axiSilent.interceptors.response.use((response) => {
  return response;
}, (error) => {
  handleErrorResponse(error);
  return Promise.reject(error);
});

export const http = axi;
export const httpSilent = axiSilent;

function handleErrorResponse(error: AxiosError) {
  if (error.response.status == 404) {
    notify(error.response.data, 'Requested item not found');
  }
  else if (error.response.status == 409) {
    notify(error.response.data, 'Request rejected');
  }
}

function notify(x: any, alt: string) {
  if (Array.isArray(x) && x.every(e => typeof e === 'string')) {
    const notifications = x.map(e => ({ text: e, severity: eSeverity.Warning }) as INotification);
    addNotification(notifications);
  }
  else {
    addNotification({ text: alt, severity: eSeverity.Warning });
  }
}