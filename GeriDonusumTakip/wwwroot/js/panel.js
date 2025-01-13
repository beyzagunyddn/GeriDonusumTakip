function updateStats(element, newValue) {
    const current = parseFloat(element.textContent);
    const target = parseFloat(newValue);
    const duration = 1000; // 1 saniye
    const steps = 60;
    const increment = (target - current) / steps;
    let step = 0;

    const timer = setInterval(() => {
        step++;
        const value = current + (increment * step);
        element.textContent = value.toFixed(1);

        if (step >= steps) {
            clearInterval(timer);
            element.textContent = target.toFixed(1);
        }
    }, duration / steps);
} 

// Anlık etki hesaplayıcı
document.addEventListener('DOMContentLoaded', function() {
    const miktarInput = document.querySelector('input[name="Miktar"]');
    const turSelect = document.querySelector('select[name="Tur"]');
    
    function hesaplaEtki() {
        const miktar = parseFloat(miktarInput.value) || 0;
        const tur = turSelect.value;
        
        let agacEtkisi = 0;
        let suEtkisi = 0;
        
        if (tur === 'Kagit') {
            agacEtkisi = miktar * 0.017; // Her kg kağıt için 0.017 ağaç
            suEtkisi = miktar * 20;      // Her kg kağıt için 20 Lt su
        }
        
        document.getElementById('agacEtkisi').textContent = agacEtkisi.toFixed(2) + ' adet';
        document.getElementById('suEtkisi').textContent = suEtkisi.toFixed(0) + ' Lt';
    }
    
    miktarInput.addEventListener('input', hesaplaEtki);
    turSelect.addEventListener('change', hesaplaEtki);
}); 