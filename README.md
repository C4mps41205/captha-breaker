/* eslint-disable no-restricted-properties */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable no-param-reassign */
import axios from 'axios';
import jwt from 'jsonwebtoken';

const api = axios.create({
  baseURL: process.env.REACT_APP_BASEURL,
});

const g = () => {
  const a: any = new Date().getTime();
  const hash = jwt.sign({ data: a }, a.toString());
  return `${a}.${hash}`;
};

api.interceptors.request.use(
  config => {
    config.headers['X-Transfacil-Hash'] = g();
    console.log(config.headers);
    return config;
  },
  error => {
    console.log(error);
    return Promise.reject(error);
  },
);

// api.defaults.headers.common['X-Transfacil-Hash'] = g();

api.interceptors.response.use(
  success => success,
  error => {
    const { ...err } = error;
    console.log({ err });
    // const hasDetailError = err.response.data.hasOwnProperty('detail');

    // if (hasDetailError) {
    //   if (
    //     err.response.data.detail === 'Credenciais de autenticação incorretas.'
    //   ) {
    //     localStorage.removeItem('@TransfacilPJ:token');
    //     localStorage.removeItem('@TransfacilPJ:token');
    //     window.location.href = '/';
    //   }
    // }
    return Promise.reject(error);
  },
);

export default api;
