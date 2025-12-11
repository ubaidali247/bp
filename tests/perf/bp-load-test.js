import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = __ENV.BP_BASE_URL || 'http://localhost:5000';
const SLEEP_SECONDS = Number(__ENV.BP_SLEEP || '1');

export const options = {
  vus: Number(__ENV.BP_VUS || '5'),
  duration: __ENV.BP_DURATION || '30s',
  thresholds: {
    http_req_duration: ['p(95)<500'],
    http_req_failed: ['rate<0.01'],
  },
};

export default function () {
  const response = http.get(`${BASE_URL}/`);

  check(response, {
    'status is 200': (r) => r.status === 200,
    'shows calculator': (r) => r.body.includes('BP Category Calculator'),
  });

  sleep(SLEEP_SECONDS);
}
